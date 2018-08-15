using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace YATTS {
    public static class Consts {
        public static int
            TRUCK_WHEEL_COUNT = 8,
            TRAILER_WHEEL_COUNT = 16,
            FWD_GEAR_COUNT = 32,
            RVS_GEAR_COUNT = 16;
        public static Brush GOLDENRODYELLOW = new SolidColorBrush(Color.FromArgb(0xff, 0xda, 0xa5, 0x20));
    }

    public static class Categories {
        public static string
            GAME_INFO = "Game info",
            POSITION = "Location",
            DRIVETRAIN = "Drivetrain",
            STEERING = "Steering",
            PERIPHERALS = "Peripherals",
            LIGHTS = "Lights",
            WEAR = "Wear",
            NAVIGATION = "Navigation",
            WHEELS = "Wheels",
            TRUCK = "Truck",
            TRAILER = "Trailer",
            JOB = "Job";
    }

    public static class Helpers {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach (var cur in enumerable) {
                action(cur);
            }
        }
    }

    public static class Converters {
        public static Dictionary<Unit, Dictionary<Unit, Func<float, float>>> ConverterDictionary = new Dictionary<Unit, Dictionary<Unit, Func<float, float>>>() {
            {
                Unit.NONE, null
            },
            {
                Unit.MS, new Dictionary<Unit, Func<float, float>>() {
                    {Unit.KMH, val => val * 3.6f },
                    {Unit.MPH, val => val * 2.23693629f }
                }
            },
            {
                Unit.PSI, new Dictionary<Unit, Func<float, float>>() {
                    {Unit.MPA, val => val * 0.00689476f },
                    {Unit.BAR, val => val * 0.06894757f }
                }
            },
            {
                Unit.C, new Dictionary<Unit, Func<float, float>>() {
                    {Unit.F, val => (val * 1.8f) + 32f },
                    {Unit.K, val => val + 273f }
                }
            },
            {
                Unit.L, new Dictionary<Unit, Func<float, float>>() {
                    {Unit.GAL, val => val * 0.26417205f }
                }
            },
            {
                Unit.KM, new Dictionary<Unit, Func<float, float>>() {
                    {Unit.MI, val => val * 0.62137119f }
                }
            },
            {
                Unit.L100KM, new Dictionary<Unit, Func<float, float>>() {
                    {Unit.MPH, val => 235.21f / val }
                }
            }
        };
    }

    public enum Unit {
        NONE,
        MS, KMH, MPH,
        PSI, MPA, BAR,
        C, F, K,
        L, GAL,
        KM, MI,
        L100KM, MPG
    }

    public enum ConvertMode {
        NONE,
        MULTIPLY,
        CHANGE_UNIT
    }

    public enum CastMode {
        NONE,
        FLOOR,
        ROUND,
        CEIL
    }

    public enum SerialOpenResults {
        OK,
        FAILED_PORTNAME,
        FAILED_NOTFOUND,
        FAILED_ALREADYUSED,
        FAILED_BAUDRATEOOR,
        FAILED_BAUDRATENULL,
        FAILED_UNKNOWN
    }
}
