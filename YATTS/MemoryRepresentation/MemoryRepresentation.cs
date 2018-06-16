using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using static YATTS.Consts;

namespace YATTS {
    public class MemoryRepresentation : INotifyPropertyChanged {
        public ObservableCollection<TelemVar> StreamedVars { get; private set; }
        public EventVariableList EventVariableList { get; private set; }

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

        public MemoryRepresentation() {
            StreamedVars = new ObservableCollection<TelemVar> {
                new U32TelemVar("plugin_version", "", "", Cats.GameInfo, 0),
                new U32TelemVar("game", "", "", Cats.GameInfo, 4),
                new U32TelemVar("sdk_version", "", "", Cats.GameInfo, 8),
                new BoolTelemVar("paused", "", "", Cats.GameInfo, 12),
                new FloatTelemVar("g_local_scale", "", "", Cats.GameInfo, 13, 8),
                new U32TelemVar("g_game_time", "", "", Cats.GameInfo, 17),
                new S32TelemVar("g_next_rest_stop", "", "", Cats.GameInfo, 21),
                new DPlacementTelemVar("t_world_placement", "", "", Cats.Position, 25),
                new FVectorTelemVar("t_local_linear_velocity", "", "", Cats.Position, 65),
                new FVectorTelemVar("t_local_angular_velocity", "", "", Cats.Position, 77),
                new FVectorTelemVar("t_local_linear_acceleration", "", "", Cats.Position, 89),
                new FVectorTelemVar("t_local_angular_acceleration", "", "", Cats.Position, 101),
                new FPlacementTelemVar("t_cabin_offset", "", "", Cats.Position, 113),
                new FVectorTelemVar("t_cabin_angular_velocity", "", "", Cats.Position, 137),
                new FVectorTelemVar("t_cabin_angular_acceleration", "", "", Cats.Position, 149),
                new FPlacementTelemVar("t_head_offset", "", "", Cats.Position, 161),
                new FloatTelemVar("t_speed", "", "", Cats.Drivetrain, 185),
                new FloatTelemVar("t_engine_rpm", "", "", Cats.Drivetrain, 189),
                new S32TelemVar("t_engine_gear", "", "", Cats.Drivetrain, 193),
                new S32TelemVar("t_displayed_gear", "", "", Cats.Drivetrain, 197),
                new FloatTelemVar("t_input_steering", "", "", Cats.Steering, 201),
                new FloatTelemVar("t_input_throttle", "", "", Cats.Steering, 205),
                new FloatTelemVar("t_input_brake", "", "", Cats.Steering, 209),
                new FloatTelemVar("t_input_clutch", "", "", Cats.Steering, 213),
                new FloatTelemVar("t_effective_steering", "", "", Cats.Steering, 217),
                new FloatTelemVar("t_effective_throttle", "", "", Cats.Steering, 221),
                new FloatTelemVar("t_effective_brake", "", "", Cats.Steering, 225),
                new FloatTelemVar("t_effective_clutch", "", "", Cats.Steering, 229),
                new FloatTelemVar("t_cruise_control", "", "", Cats.Steering, 233),
                new U32TelemVar("t_hshifter_slot", "", "", Cats.Steering, 237),
                new BoolTelemVar("t_parking_brake", "", "", Cats.Steering, 241),
                new BoolTelemVar("t_motor_brake", "", "", Cats.Steering, 242),
                new U32TelemVar("t_retarder_level", "", "", Cats.Steering, 243),
                new FloatTelemVar("t_brake_air_pressure", "", "", Cats.Steering, 247),
                new BoolTelemVar("t_brake_air_pressure_warning", "", "", Cats.Steering, 251),
                new BoolTelemVar("t_brake_air_pressure_emergency", "", "", Cats.Steering, 252),
                new FloatTelemVar("t_brake_temperature", "", "", Cats.Steering, 253),
                new FloatTelemVar("t_fuel", "", "", Cats.Peripherals, 257),
                new BoolTelemVar("t_fuel_warning", "", "", Cats.Peripherals, 261),
                new FloatTelemVar("t_fuel_average_consumption", "", "", Cats.Peripherals, 262),
                new FloatTelemVar("t_fuel_range", "", "", Cats.Peripherals, 266),
                new FloatTelemVar("t_adblue", "", "", Cats.Peripherals, 270),
                new BoolTelemVar("t_adblue_warning", "", "", Cats.Peripherals, 274),
                new FloatTelemVar("t_adblue_average_consumption", "", "", Cats.Peripherals, 275),
                new FloatTelemVar("t_oil_pressure", "", "", Cats.Peripherals, 279),
                new BoolTelemVar("t_oil_pressure_warning", "", "", Cats.Peripherals, 283),
                new FloatTelemVar("t_oil_temperature", "", "", Cats.Peripherals, 284),
                new FloatTelemVar("t_water_temperature", "", "", Cats.Peripherals, 288),
                new BoolTelemVar("t_water_temperature_warning", "", "", Cats.Peripherals, 292),
                new FloatTelemVar("t_battery_voltage", "", "", Cats.Peripherals, 293),
                new BoolTelemVar("t_battery_voltage_warning", "", "", Cats.Peripherals, 297),
                new BoolTelemVar("t_electric_enabled", "", "", Cats.Peripherals, 298),
                new BoolTelemVar("t_engine_enabled", "", "", Cats.Drivetrain, 299),
                new BoolTelemVar("t_lblinker", "", "", Cats.Lights, 300),
                new BoolTelemVar("t_rblinker", "", "", Cats.Lights, 301),
                new BoolTelemVar("t_light_lblinker", "", "", Cats.Lights, 302),
                new BoolTelemVar("t_light_rblinker", "", "", Cats.Lights, 303),
                new BoolTelemVar("t_light_parking", "", "", Cats.Lights, 304),
                new BoolTelemVar("t_light_low_beam", "", "", Cats.Lights, 305),
                new BoolTelemVar("t_light_high_beam", "", "", Cats.Lights, 306),
                new U32TelemVar("t_light_aux_front", "", "", Cats.Lights, 307),
                new U32TelemVar("t_light_aux_roof", "", "", Cats.Lights, 311),
                new BoolTelemVar("t_light_beacon", "", "", Cats.Lights, 315),
                new BoolTelemVar("t_light_brake", "", "", Cats.Lights, 316),
                new BoolTelemVar("t_light_reverse", "", "", Cats.Lights, 317),
                new BoolTelemVar("t_wipers", "", "", Cats.Peripherals, 318),
                new FloatTelemVar("t_dashboard_backlight", "", "", Cats.Peripherals, 319),
                new FloatTelemVar("t_wear_engine", "", "", Cats.Wear, 323),
                new FloatTelemVar("t_wear_transmission", "", "", Cats.Wear, 327),
                new FloatTelemVar("t_wear_cabin", "", "", Cats.Wear, 331),
                new FloatTelemVar("t_wear_chassis", "", "", Cats.Wear, 335),
                new FloatTelemVar("t_wear_wheels", "", "", Cats.Wear, 339),
                new FloatTelemVar("t_odometer", "", "", Cats.Peripherals, 343),
                new FloatTelemVar("t_navigation_distance", "", "", Cats.Navigation, 347),
                new FloatTelemVar("t_navigation_time", "", "", Cats.Navigation, 351),
                new FloatTelemVar("t_navigation_speed_limit", "", "", Cats.Navigation, 355),
                new FloatTelemVar("t_wheel_susp_deflection", "", "", Cats.Wheels, 359, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("t_wheel_on_ground", "", "", Cats.Wheels, 391, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_velocity", "", "", Cats.Wheels, 399, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_steering", "", "", Cats.Wheels, 431, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_rotation", "", "", Cats.Wheels, 463, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_lift", "", "", Cats.Wheels, 395, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_lift_offset", "", "", Cats.Wheels, 527, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("r_connected", "", "", Cats.Trailer, 559),
                new DPlacementTelemVar("r_world_placement", "", "", Cats.Trailer, 560),
                new FVectorTelemVar("r_local_linear_velocity", "", "", Cats.Trailer, 600),
                new FVectorTelemVar("r_local_angular_velocity", "", "", Cats.Trailer, 612),
                new FVectorTelemVar("r_local_linear_acceleration", "", "", Cats.Trailer, 624),
                new FVectorTelemVar("r_local_angular_acceleration", "", "", Cats.Trailer, 636),
                new FloatTelemVar("r_wear_chassis", "", "", Cats.Trailer, 648),
                new FloatTelemVar("r_wheel_susp_deflection", "", "", Cats.Trailer, 652, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("r_wheel_on_ground", "", "", Cats.Trailer, 716, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("r_wheel_velocity", "", "", Cats.Trailer, 732, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("r_wheel_steering", "", "", Cats.Trailer, 796, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("r_wheel_rotation", "", "", Cats.Trailer, 860, TRAILER_WHEEL_COUNT)
            };

            ObservableCollection<TelemVar> EventVars = new ObservableCollection<TelemVar>() {
                new ASCIITelemVar("ct_brand_id", "", "", Cats.Truck, 927),
                new ASCIITelemVar("ct_brand", "", "", Cats.Truck, 991),
                new ASCIITelemVar("ct_id", "", "", Cats.Truck, 1055),
                new ASCIITelemVar("ct_name", "", "", Cats.Truck, 1119),
                new FloatTelemVar("ct_fuel_capacity", "", "", Cats.Truck, 1119),
                new FloatTelemVar("ct_fuel_warning_factor", "", "", Cats.Truck, 1183),
                new FloatTelemVar("ct_adblue_capacity", "", "", Cats.Truck, 1187),
                new FloatTelemVar("ct_adblue_warning_factor", "", "", Cats.Truck, 1195),
                new FloatTelemVar("ct_air_pressure_warning", "", "", Cats.Truck, 1199),
                new FloatTelemVar("ct_air_pressure_emergency", "", "", Cats.Truck, 1203),
                new FloatTelemVar("ct_oil_pressure_warning", "", "", Cats.Truck, 1207),
                new FloatTelemVar("ct_water_temperature_warning", "", "", Cats.Truck, 1211),
                new FloatTelemVar("ct_battery_voltage_warning", "", "", Cats.Truck, 1215),
                new FloatTelemVar("ct_rpm_limit", "", "", Cats.Truck, 1219),
                new U32TelemVar("ct_forward_gear_count", "", "", Cats.Truck, 1223),
                new U32TelemVar("ct_reverse_gear_count", "", "", Cats.Truck, 1227),
                new FloatTelemVar("ct_differential_ratio", "", "", Cats.Truck, 1231),
                new U32TelemVar("ct_retarder_step_count", "", "", Cats.Truck, 1235),
                new FVectorTelemVar("ct_cabin_position", "", "", Cats.Truck, 1239),
                new FVectorTelemVar("ct_head_position", "", "", Cats.Truck, 1251),
                new FVectorTelemVar("ct_hook_position", "", "", Cats.Truck, 1263),
                new U32TelemVar("ct_wheel_count", "", "", Cats.Truck, 1275),
                new FVectorTelemVar("ct_wheel_position", "", "", Cats.Truck, 1279, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_steerable", "", "", Cats.Truck, 1375, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_simulated", "", "", Cats.Truck, 1383, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("ct_wheel_radius", "", "", Cats.Truck, 1391, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_powered", "", "", Cats.Truck, 1423, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_liftable", "", "", Cats.Truck, 1431, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("ct_forward_ratio", "", "", Cats.Truck, 1439, FWD_GEAR_COUNT),
                new FloatTelemVar("ct_reverse_ratio", "", "", Cats.Truck, 1567, RVS_GEAR_COUNT),
                new ASCIITelemVar("cr_id", "", "", Cats.Trailer, 1631),
                new ASCIITelemVar("cr_cargo_accessory_id", "", "", Cats.Trailer, 1695),
                new FVectorTelemVar("cr_hook_position", "", "", Cats.Trailer, 1759),
                new U32TelemVar("cr_wheel_count", "", "", Cats.Trailer, 1771),
                new FVectorTelemVar("cr_wheel_position", "", "", Cats.Trailer, 1775, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_steerable", "", "", Cats.Trailer, 1967, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_simulated", "", "", Cats.Trailer, 1983, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("cr_wheel_radius", "", "", Cats.Trailer, 1999, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_powered", "", "", Cats.Trailer, 2063, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_liftable", "", "", Cats.Trailer, 2079, TRAILER_WHEEL_COUNT),
                new ASCIITelemVar("cj_cargo_id", "", "", Cats.Job, 2095),
                new ASCIITelemVar("cj_cargo", "", "", Cats.Job, 2159),
                new FloatTelemVar("cj_cargo_mass", "", "", Cats.Job, 2223),
                new ASCIITelemVar("cj_destination_city_id", "", "", Cats.Job, 2227),
                new ASCIITelemVar("cj_destination_city", "", "", Cats.Job, 2291),
                new ASCIITelemVar("cj_destination_company_id", "", "", Cats.Job, 2355),
                new ASCIITelemVar("cj_destination_company", "", "", Cats.Job, 2419),
                new ASCIITelemVar("cj_source_city_id", "", "", Cats.Job, 2483),
                new ASCIITelemVar("cj_source_city", "", "", Cats.Job, 2547),
                new ASCIITelemVar("cj_source_company_id", "", "", Cats.Job, 2611),
                new ASCIITelemVar("cj_source_company", "", "", Cats.Job, 2675),
                new U64TelemVar("cj_income", "", "", Cats.Job, 2739),
                new U32TelemVar("cj_delivery_time", "", "", Cats.Job, 2747)
            };

            U8TelemVar truckDataMarker = new U8TelemVar(null, null, null, null, 924);
            U8TelemVar trailerDataMarker = new U8TelemVar(null, null, null, null, 925);
            U8TelemVar jobDataMarker = new U8TelemVar(null, null, null, null, 926);

            this.EventVariableList = new EventVariableList(EventVars, truckDataMarker, trailerDataMarker, jobDataMarker);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string sender) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sender));
        }
    }
}
