using System;

namespace YATTS {
    public static class Consts {
        public static int
            TRUCK_WHEEL_COUNT = 8,
            TRAILER_WHEEL_COUNT = 16,
            FWD_GEAR_COUNT = 16,
            RVS_GEAR_COUNT = 32;
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

    public static class Converters {
        public static Func<float, float> MStoKMH = val => val * 3.6f;
        public static Func<float, float> MStoMPH = val => val * 2.23693629f;

        public static Func<float, float> PSItoMPA = val => val * 0.00689476f;
        public static Func<float, float> PSItoBAR = val => val * 0.06894757f;

        public static Func<float, float> CtoF = val => (val * 1.8f) + 32f;
        public static Func<float, float> CtoK = val => val + 273f;

        public static Func<float, float> LtoGAL = val => val * 0.26417205f;

        public static Func<float, float> KMtoMI = val => val * 0.62137119f;
        public static Func<float, float> L100KMtoMPG = val => 235.21f / val;
    }

    public enum CastMode {
        NONE,
        FLOOR,
        ROUND,
        CEIL
    }

    public enum Unit {
        NONE,
        MS,
        PSI,
        C,
        L,
        KM,
        L100KM
    }
}
