using UnityEngine;
using System;

public static class SeededRNG {
    private static System.Random rng = new System.Random();
    private static int seed = 0;
    private static bool isSetSeed = false;


    // State methods

    public static void Reset() {
        SetSeedInternal(UnityEngine.Random.Range(0, int.MaxValue));
        isSetSeed = false;
    }

    public static void SetSeed(int seed) {
        SetSeedInternal(seed);
        isSetSeed = true;
    }

    private static void SetSeedInternal(int seed) {
        SeededRNG.seed = seed;
        rng = new System.Random(seed);
    }

    public static bool IsSetSeed() {
        return isSetSeed;
    }

    public static int GetSeed() {
        return seed;
    }


    // Random methods

    public static int Range(int min, int max) {
        return rng.Next(min, max);
    }

    public static int Range(int max) {
        return rng.Next(max);
    }

    public static float Range(float min, float max) {
        return Value * (max - min) + min;
    }

    public static float Range(float max) {
        return Value * max;
    }

    public static float Value => (float) rng.NextDouble();
}
