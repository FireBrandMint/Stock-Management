using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

public class CacheThreadsafe<TKey, TValue, TVersion>
where TKey : notnull
where TVersion : INumber<TVersion>
{
    int Size;
    int Mask;
    long RingAddIdx = 0;
    Dictionary<TKey, int> Dict;
    TVersion[] Versions;
    TKey[] Keys;
    TValue[] Values;
    ReaderWriterLockSlim Lock;
    public CacheThreadsafe(int bitSize)
    {
        if(bitSize <= 0 || bitSize > 31)
            throw new ArgumentException($"bitSize too big or too small: {bitSize}");
        Size = 1 << bitSize;
        Mask = Size - 1;
        Dict = new Dictionary<TKey, int>();
        Versions = new TVersion[Size];
        Keys = new TKey[Size];
        Values = new TValue[Size];
        Lock = new ReaderWriterLockSlim();
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue? value)
    {
        bool result = false;
        Lock.EnterReadLock();
        try
        {
            int idx;
            if(Dict.TryGetValue(key, out idx))
            {
                result = true;
                value = Values[idx];
            }
            else
                value = default;
        }
        finally
        {
            Lock.ExitReadLock();
        }

        return result;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue? value, [MaybeNullWhen(false)] out TVersion? version)
    {
        bool result = false;
        Lock.EnterReadLock();
        try
        {
            int idx;
            if(Dict.TryGetValue(key, out idx))
            {
                result = true;
                value = Values[idx];
                version = Versions[idx];
            }
            else
            {
                value = default;
                version = default;
            }
        }
        finally
        {
            Lock.ExitReadLock();
        }

        return result;
    }

    public void Push(TKey key, TValue value, TVersion version)
    {
        try
        {
            Lock.EnterUpgradeableReadLock();

            if(Dict.TryGetValue(key, out var subj_idx) && Versions[subj_idx] < version)
            {
                Lock.EnterWriteLock();
                
                TryAddUnsafe(key, value, version);

                Lock.ExitWriteLock();
            }
            else
            {
                Lock.EnterWriteLock();

                TryAddUnsafe(key, value, version);

                Lock.ExitWriteLock();
            }
        }
        finally
        {
            Lock.ExitUpgradeableReadLock();
        }
    }

    private void TryAddUnsafe(TKey key, TValue value, TVersion version)
    {
        var versions = Versions;
        var keys = Keys;
        var values = Values;
        if(Dict.TryGetValue(key, out var subj_idx))
        {
            if(Versions[subj_idx] < version)
            {
                Dict.Remove(keys[subj_idx]);
                Dict.Add(key, subj_idx);
                versions[subj_idx] = version;
                keys[subj_idx] = key;
                values[subj_idx] = value;
            }
        }
        else
        {
            int mask = Mask;
            long last_ring_add = RingAddIdx++;
            //Replace idx
            int rep_idx = (int)(last_ring_add & mask);

            //Reached the max count and is looping around
            if(last_ring_add > Size)
            {
                Dict.Remove(keys[rep_idx]);
            }

            Dict.Add(key, rep_idx);
            versions[rep_idx] = version;
            keys[rep_idx] = key;
            values[rep_idx] = value;
        }
    }
}