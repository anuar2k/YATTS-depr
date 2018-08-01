using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.MemoryMappedFiles;
using static YATTS.Categories;
using static YATTS.Consts;

namespace YATTS {
    public class MemoryRepresentation : INotifyPropertyChanged {
        public ObservableCollection<TelemVar> StreamedVars { get; private set; }
        public EventVariableList EventVariableList { get; private set; }
        public MemoryMappedFile MMF { get; private set; }
        public MemoryMappedViewAccessor MMVA { get; private set; }

        private TelemVar _Selected;
        public TelemVar Selected {
            get {
                return _Selected;
            }
            set {
                if (value != _Selected) {
                    _Selected = value;
                    OnPropertyChanged(nameof(Selected));
                    SelectedFloat = value as FloatTelemVar;
                    SelectedStringable = value as StringableTelemVar;
                }
            }
        }

        private FloatTelemVar _SelectedFloat;
        public FloatTelemVar SelectedFloat {
            get {
                return _SelectedFloat;
            }
            set {
                if (value != _SelectedFloat) {
                    _SelectedFloat = value;
                    OnPropertyChanged(nameof(SelectedFloat));
                }
            }
        }

        private StringableTelemVar _SelectedStringable;
        public StringableTelemVar SelectedStringable {
            get {
                return _SelectedStringable;
            }
            set {
                if (value != _SelectedStringable) {
                    _SelectedStringable = value;
                    OnPropertyChanged(nameof(SelectedStringable));
                }
            }
        }

        public MemoryRepresentation() {
            StreamedVars = new ObservableCollection<TelemVar> {
                new U32TelemVar("plugin_version", "", "", GAME_INFO, 0),
                new U32TelemVar("game", "", "", GAME_INFO, 4),
                new U32TelemVar("sdk_version", "", "", GAME_INFO, 8),
                new BoolTelemVar("paused", "", "", GAME_INFO, 12),
                new FloatTelemVar("g_local_scale", "", "", GAME_INFO, 13, Unit.NONE),
                new U32TelemVar("g_game_time", "", "", GAME_INFO, 17),
                new S32TelemVar("g_next_rest_stop", "", "", GAME_INFO, 21),
                new DPlacementTelemVar("t_world_placement", "", "", POSITION, 25),
                new FVectorTelemVar("t_local_linear_velocity", "", "", POSITION, 65),
                new FVectorTelemVar("t_local_angular_velocity", "", "", POSITION, 77),
                new FVectorTelemVar("t_local_linear_acceleration", "", "", POSITION, 89),
                new FVectorTelemVar("t_local_angular_acceleration", "", "", POSITION, 101),
                new FPlacementTelemVar("t_cabin_offset", "", "", POSITION, 113),
                new FVectorTelemVar("t_cabin_angular_velocity", "", "", POSITION, 137),
                new FVectorTelemVar("t_cabin_angular_acceleration", "", "", POSITION, 149),
                new FPlacementTelemVar("t_head_offset", "", "", POSITION, 161),
                new FloatTelemVar("t_speed", "", "", DRIVETRAIN, 185, Unit.MS),
                new FloatTelemVar("t_engine_rpm", "", "", DRIVETRAIN, 189, Unit.NONE),
                new S32TelemVar("t_engine_gear", "", "", DRIVETRAIN, 193),
                new S32TelemVar("t_displayed_gear", "", "", DRIVETRAIN, 197),
                new FloatTelemVar("t_input_steering", "", "", STEERING, 201, Unit.NONE),
                new FloatTelemVar("t_input_throttle", "", "", STEERING, 205, Unit.NONE),
                new FloatTelemVar("t_input_brake", "", "", STEERING, 209, Unit.NONE),
                new FloatTelemVar("t_input_clutch", "", "", STEERING, 213, Unit.NONE),
                new FloatTelemVar("t_effective_steering", "", "", STEERING, 217, Unit.NONE),
                new FloatTelemVar("t_effective_throttle", "", "", STEERING, 221, Unit.NONE),
                new FloatTelemVar("t_effective_brake", "", "", STEERING, 225, Unit.NONE),
                new FloatTelemVar("t_effective_clutch", "", "", STEERING, 229, Unit.NONE),
                new FloatTelemVar("t_cruise_control", "", "", STEERING, 233, Unit.MS),
                new U32TelemVar("t_hshifter_slot", "", "", STEERING, 237),
                new BoolTelemVar("t_parking_brake", "", "", STEERING, 241),
                new BoolTelemVar("t_motor_brake", "", "", STEERING, 242),
                new U32TelemVar("t_retarder_level", "", "", STEERING, 243),
                new FloatTelemVar("t_brake_air_pressure", "", "", STEERING, 247, Unit.PSI),
                new BoolTelemVar("t_brake_air_pressure_warning", "", "", STEERING, 251),
                new BoolTelemVar("t_brake_air_pressure_emergency", "", "", STEERING, 252),
                new FloatTelemVar("t_brake_temperature", "", "", STEERING, 253, Unit.C),
                new FloatTelemVar("t_fuel", "", "", PERIPHERALS, 257, Unit.L),
                new BoolTelemVar("t_fuel_warning", "", "", PERIPHERALS, 261),
                new FloatTelemVar("t_fuel_average_consumption", "", "", PERIPHERALS, 262, Unit.L100KM),
                new FloatTelemVar("t_fuel_range", "", "", PERIPHERALS, 266, Unit.KM), //not sure about this one
                new FloatTelemVar("t_adblue", "", "", PERIPHERALS, 270, Unit.L),
                new BoolTelemVar("t_adblue_warning", "", "", PERIPHERALS, 274),
                new FloatTelemVar("t_adblue_average_consumption", "", "", PERIPHERALS, 275, Unit.L100KM),
                new FloatTelemVar("t_oil_pressure", "", "", PERIPHERALS, 279, Unit.PSI),
                new BoolTelemVar("t_oil_pressure_warning", "", "", PERIPHERALS, 283),
                new FloatTelemVar("t_oil_temperature", "", "", PERIPHERALS, 284, Unit.C),
                new FloatTelemVar("t_water_temperature", "", "", PERIPHERALS, 288, Unit.C),
                new BoolTelemVar("t_water_temperature_warning", "", "", PERIPHERALS, 292),
                new FloatTelemVar("t_battery_voltage", "", "", PERIPHERALS, 293, Unit.NONE),
                new BoolTelemVar("t_battery_voltage_warning", "", "", PERIPHERALS, 297),
                new BoolTelemVar("t_electric_enabled", "", "", PERIPHERALS, 298),
                new BoolTelemVar("t_engine_enabled", "", "", DRIVETRAIN, 299),
                new BoolTelemVar("t_lblinker", "", "", LIGHTS, 300),
                new BoolTelemVar("t_rblinker", "", "", LIGHTS, 301),
                new BoolTelemVar("t_light_lblinker", "", "", LIGHTS, 302),
                new BoolTelemVar("t_light_rblinker", "", "", LIGHTS, 303),
                new BoolTelemVar("t_light_parking", "", "", LIGHTS, 304),
                new BoolTelemVar("t_light_low_beam", "", "", LIGHTS, 305),
                new BoolTelemVar("t_light_high_beam", "", "", LIGHTS, 306),
                new U32TelemVar("t_light_aux_front", "", "", LIGHTS, 307),
                new U32TelemVar("t_light_aux_roof", "", "", LIGHTS, 311),
                new BoolTelemVar("t_light_beacon", "", "", LIGHTS, 315),
                new BoolTelemVar("t_light_brake", "", "", LIGHTS, 316),
                new BoolTelemVar("t_light_reverse", "", "", LIGHTS, 317),
                new BoolTelemVar("t_wipers", "", "", PERIPHERALS, 318),
                new FloatTelemVar("t_dashboard_backlight", "", "", PERIPHERALS, 319, Unit.NONE),
                new FloatTelemVar("t_wear_engine", "", "", WEAR, 323, Unit.NONE),
                new FloatTelemVar("t_wear_transmission", "", "", WEAR, 327, Unit.NONE),
                new FloatTelemVar("t_wear_cabin", "", "", WEAR, 331, Unit.NONE),
                new FloatTelemVar("t_wear_chassis", "", "", WEAR, 335, Unit.NONE),
                new FloatTelemVar("t_wear_wheels", "", "", WEAR, 339, Unit.NONE),
                new FloatTelemVar("t_odometer", "", "", PERIPHERALS, 343, Unit.NONE),
                new FloatTelemVar("t_navigation_distance", "", "", NAVIGATION, 347, Unit.KM), //not sure about this one
                new FloatTelemVar("t_navigation_time", "", "", NAVIGATION, 351, Unit.NONE),
                new FloatTelemVar("t_navigation_speed_limit", "", "", NAVIGATION, 355, Unit.MS),
                new FloatTelemVar("t_wheel_susp_deflection", "", "", WHEELS, 359, Unit.NONE, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("t_wheel_on_ground", "", "", WHEELS, 391, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_velocity", "", "", WHEELS, 399, Unit.NONE, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_steering", "", "", WHEELS, 431, Unit.NONE, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_rotation", "", "", WHEELS, 463, Unit.NONE, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_lift", "", "", WHEELS, 395, Unit.NONE, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_lift_offset", "", "", WHEELS, 527, Unit.NONE, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("r_connected", "", "", TRAILER, 559),
                new DPlacementTelemVar("r_world_placement", "", "", TRAILER, 560),
                new FVectorTelemVar("r_local_linear_velocity", "", "", TRAILER, 600),
                new FVectorTelemVar("r_local_angular_velocity", "", "", TRAILER, 612),
                new FVectorTelemVar("r_local_linear_acceleration", "", "", TRAILER, 624),
                new FVectorTelemVar("r_local_angular_acceleration", "", "", TRAILER, 636),
                new FloatTelemVar("r_wear_chassis", "", "", TRAILER, 648, Unit.NONE),
                new FloatTelemVar("r_wheel_susp_deflection", "", "", TRAILER, 652, Unit.NONE, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("r_wheel_on_ground", "", "", TRAILER, 716, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("r_wheel_velocity", "", "", TRAILER, 732, Unit.NONE, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("r_wheel_steering", "", "", TRAILER, 796, Unit.NONE, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("r_wheel_rotation", "", "", TRAILER, 860, Unit.NONE, TRAILER_WHEEL_COUNT)
            };

            ObservableCollection<TelemVar> EventVars = new ObservableCollection<TelemVar>() {
                new StringTelemVar("ct_brand_id", "", "", TRUCK, 927),
                new StringTelemVar("ct_brand", "", "", TRUCK, 991),
                new StringTelemVar("ct_id", "", "", TRUCK, 1055),
                new StringTelemVar("ct_name", "", "", TRUCK, 1119),
                new FloatTelemVar("ct_fuel_capacity", "", "", TRUCK, 1183, Unit.L),
                new FloatTelemVar("ct_fuel_warning_factor", "", "", TRUCK, 1187, Unit.L),
                new FloatTelemVar("ct_adblue_capacity", "", "", TRUCK, 1191, Unit.L),
                new FloatTelemVar("ct_adblue_warning_factor", "", "", TRUCK, 1195, Unit.L),
                new FloatTelemVar("ct_air_pressure_warning", "", "", TRUCK, 1199, Unit.PSI),
                new FloatTelemVar("ct_air_pressure_emergency", "", "", TRUCK, 1203, Unit.PSI),
                new FloatTelemVar("ct_oil_pressure_warning", "", "", TRUCK, 1207, Unit.PSI),
                new FloatTelemVar("ct_water_temperature_warning", "", "", TRUCK, 1211, Unit.C),
                new FloatTelemVar("ct_battery_voltage_warning", "", "", TRUCK, 1215, Unit.NONE),
                new FloatTelemVar("ct_rpm_limit", "", "", TRUCK, 1219, Unit.NONE),
                new U32TelemVar("ct_forward_gear_count", "", "", TRUCK, 1223),
                new U32TelemVar("ct_reverse_gear_count", "", "", TRUCK, 1227),
                new FloatTelemVar("ct_differential_ratio", "", "", TRUCK, 1231, Unit.NONE),
                new U32TelemVar("ct_retarder_step_count", "", "", TRUCK, 1235),
                new FVectorTelemVar("ct_cabin_position", "", "", TRUCK, 1239),
                new FVectorTelemVar("ct_head_position", "", "", TRUCK, 1251),
                new FVectorTelemVar("ct_hook_position", "", "", TRUCK, 1263),
                new U32TelemVar("ct_wheel_count", "", "", TRUCK, 1275),
                new FVectorTelemVar("ct_wheel_position", "", "", TRUCK, 1279, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_steerable", "", "", TRUCK, 1375, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_simulated", "", "", TRUCK, 1383, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("ct_wheel_radius", "", "", TRUCK, 1391, Unit.NONE, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_powered", "", "", TRUCK, 1423, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_liftable", "", "", TRUCK, 1431, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("ct_forward_ratio", "", "", TRUCK, 1439, Unit.NONE, FWD_GEAR_COUNT),
                new FloatTelemVar("ct_reverse_ratio", "", "", TRUCK, 1567, Unit.NONE, RVS_GEAR_COUNT),
                new StringTelemVar("cr_id", "", "", TRAILER, 1631),
                new StringTelemVar("cr_cargo_accessory_id", "", "", TRAILER, 1695),
                new FVectorTelemVar("cr_hook_position", "", "", TRAILER, 1759),
                new U32TelemVar("cr_wheel_count", "", "", TRAILER, 1771),
                new FVectorTelemVar("cr_wheel_position", "", "", TRAILER, 1775, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_steerable", "", "", TRAILER, 1967, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_simulated", "", "", TRAILER, 1983, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("cr_wheel_radius", "", "", TRAILER, 1999, Unit.NONE, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_powered", "", "", TRAILER, 2063, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_liftable", "", "", TRAILER, 2079, TRAILER_WHEEL_COUNT),
                new StringTelemVar("cj_cargo_id", "", "", JOB, 2095),
                new StringTelemVar("cj_cargo", "", "", JOB, 2159),
                new FloatTelemVar("cj_cargo_mass", "", "", JOB, 2223, Unit.NONE), //not sure bout this one - need a mass unit
                new StringTelemVar("cj_destination_city_id", "", "", JOB, 2227),
                new StringTelemVar("cj_destination_city", "", "", JOB, 2291),
                new StringTelemVar("cj_destination_company_id", "", "", JOB, 2355),
                new StringTelemVar("cj_destination_company", "", "", JOB, 2419),
                new StringTelemVar("cj_source_city_id", "", "", JOB, 2483),
                new StringTelemVar("cj_source_city", "", "", JOB, 2547),
                new StringTelemVar("cj_source_company_id", "", "", JOB, 2611),
                new StringTelemVar("cj_source_company", "", "", JOB, 2675),
                new U64TelemVar("cj_income", "", "", JOB, 2739),
                new U32TelemVar("cj_delivery_time", "", "", JOB, 2747)
            };

            U8TelemVar truckDataMarker = new U8TelemVar(null, null, null, null, 924);
            U8TelemVar trailerDataMarker = new U8TelemVar(null, null, null, null, 925);
            U8TelemVar jobDataMarker = new U8TelemVar(null, null, null, null, 926);

            EventVariableList = new EventVariableList(EventVars, truckDataMarker, trailerDataMarker, jobDataMarker);
        }

        public void Hook() {
            MMF = MemoryMappedFile.OpenExisting("Local\\YATTS_MMF");
            MMVA = MMF.CreateViewAccessor();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string sender) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sender));
        }
    }
}
