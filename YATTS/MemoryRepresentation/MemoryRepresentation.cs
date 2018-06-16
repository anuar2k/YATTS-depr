﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using static YATTS.Consts;

namespace YATTS {
    public class MemoryRepresentation : INotifyPropertyChanged {
        public ObservableCollection<TelemVar> StreamedVars { get; private set; }
        public EventVariableList EventVars { get; private set; }

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
                new U32TelemVar("plugin_version", "", "", "", 0),
                new U32TelemVar("game", "", "", "", 4),
                new U32TelemVar("sdk_version", "", "", "", 8),
                new BoolTelemVar("paused", "", "", "", 12),
                new FloatTelemVar("g_local_scale", "", "", "", 13, 8),
                new U32TelemVar("g_game_time", "", "", "", 17),
                new S32TelemVar("g_next_rest_stop", "", "", "", 21),
                new DPlacementTelemVar("t_world_placement", "", "", "", 25),
                new FVectorTelemVar("t_local_linear_velocity", "", "", "", 65),
                new FVectorTelemVar("t_local_angular_velocity", "", "", "", 77),
                new FVectorTelemVar("t_local_linear_acceleration", "", "", "", 89),
                new FVectorTelemVar("t_local_angular_acceleration", "", "", "", 101),
                new FPlacementTelemVar("t_cabin_offset", "", "", "", 113),
                new FVectorTelemVar("t_cabin_angular_velocity", "", "", "", 137),
                new FVectorTelemVar("t_cabin_angular_acceleration", "", "", "", 149),
                new FPlacementTelemVar("t_head_offset", "", "", "", 161),
                new FloatTelemVar("t_speed", "", "", "", 185),
                new FloatTelemVar("t_engine_rpm", "", "", "", 189),
                new S32TelemVar("t_engine_gear", "", "", "", 193),
                new S32TelemVar("t_displayed_gear", "", "", "", 197),
                new FloatTelemVar("t_input_steering", "", "", "", 201),
                new FloatTelemVar("t_input_throttle", "", "", "", 205),
                new FloatTelemVar("t_input_brake", "", "", "", 209),
                new FloatTelemVar("t_input_clutch", "", "", "", 213),
                new FloatTelemVar("t_effective_steering", "", "", "", 217),
                new FloatTelemVar("t_effective_throttle", "", "", "", 221),
                new FloatTelemVar("t_effective_brake", "", "", "", 225),
                new FloatTelemVar("t_effective_clutch", "", "", "", 229),
                new FloatTelemVar("t_cruise_control", "", "", "", 233),
                new U32TelemVar("t_hshifter_slot", "", "", "", 237),
                new BoolTelemVar("t_parking_brake", "", "", "", 241),
                new BoolTelemVar("t_motor_brake", "", "", "", 242),
                new U32TelemVar("t_retarder_level", "", "", "", 243),
                new FloatTelemVar("t_brake_air_pressure", "", "", "", 247),
                new BoolTelemVar("t_brake_air_pressure_warning", "", "", "", 251),
                new BoolTelemVar("t_brake_air_pressure_emergency", "", "", "", 252),
                new FloatTelemVar("t_brake_temperature", "", "", "", 253),
                new FloatTelemVar("t_fuel", "", "", "", 257),
                new BoolTelemVar("t_fuel_warning", "", "", "", 261),
                new FloatTelemVar("t_fuel_average_consumption", "", "", "", 262),
                new FloatTelemVar("t_fuel_range", "", "", "", 266),
                new FloatTelemVar("t_adblue", "", "", "", 270),
                new BoolTelemVar("t_adblue_warning", "", "", "", 274),
                new FloatTelemVar("t_adblue_average_consumption", "", "", "", 275),
                new FloatTelemVar("t_oil_pressure", "", "", "", 279),
                new BoolTelemVar("t_oil_pressure_warning", "", "", "", 283),
                new FloatTelemVar("t_oil_temperature", "", "", "", 284),
                new FloatTelemVar("t_water_temperature", "", "", "", 288),
                new BoolTelemVar("t_water_temperature_warning", "", "", "", 292),
                new FloatTelemVar("t_battery_voltage", "", "", "", 293),
                new BoolTelemVar("t_battery_voltage_warning", "", "", "", 297),
                new BoolTelemVar("t_electric_enabled", "", "", "", 298),
                new BoolTelemVar("t_engine_enabled", "", "", "", 299),
                new BoolTelemVar("t_lblinker", "", "", "", 300),
                new BoolTelemVar("t_rblinker", "", "", "", 301),
                new BoolTelemVar("t_light_lblinker", "", "", "", 302),
                new BoolTelemVar("t_light_rblinker", "", "", "", 303),
                new BoolTelemVar("t_light_parking", "", "", "", 304),
                new BoolTelemVar("t_light_low_beam", "", "", "", 305),
                new BoolTelemVar("t_light_high_beam", "", "", "", 306),
                new U32TelemVar("t_light_aux_front", "", "", "", 307),
                new U32TelemVar("t_light_aux_roof", "", "", "", 311),
                new BoolTelemVar("t_light_beacon", "", "", "", 315),
                new BoolTelemVar("t_light_brake", "", "", "", 316),
                new BoolTelemVar("t_light_reverse", "", "", "", 317),
                new BoolTelemVar("t_wipers", "", "", "", 318),
                new FloatTelemVar("t_dashboard_backlight", "", "", "", 319),
                new FloatTelemVar("t_wear_engine", "", "", "", 323),
                new FloatTelemVar("t_wear_transmission", "", "", "", 327),
                new FloatTelemVar("t_wear_cabin", "", "", "", 331),
                new FloatTelemVar("t_wear_chassis", "", "", "", 335),
                new FloatTelemVar("t_wear_wheels", "", "", "", 339),
                new FloatTelemVar("t_odometer", "", "", "", 343),
                new FloatTelemVar("t_navigation_distance", "", "", "", 347),
                new FloatTelemVar("t_navigation_time", "", "", "", 351),
                new FloatTelemVar("t_navigation_speed_limit", "", "", "", 355),
                new FloatTelemVar("t_wheel_susp_deflection", "", "", "", 359, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("t_wheel_on_ground", "", "", "", 391, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_velocity", "", "", "", 399, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_steering", "", "", "", 431, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_rotation", "", "", "", 463, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_lift", "", "", "", 395, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("t_wheel_lift_offset", "", "", "", 527, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("r_connected", "", "", "", 559),
                new DPlacementTelemVar("r_world_placement", "", "", "", 560),
                new FVectorTelemVar("r_local_linear_velocity", "", "", "", 600),
                new FVectorTelemVar("r_local_angular_velocity", "", "", "", 612),
                new FVectorTelemVar("r_local_linear_acceleration", "", "", "", 624),
                new FVectorTelemVar("r_local_angular_acceleration", "", "", "", 636),
                new FloatTelemVar("r_wear_chassis", "", "", "", 648),
                new FloatTelemVar("r_wheel_susp_deflection", "", "", "", 652, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("r_wheel_on_ground", "", "", "", 716, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("r_wheel_velocity", "", "", "", 732, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("r_wheel_steering", "", "", "", 796, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("r_wheel_rotation", "", "", "", 860, TRAILER_WHEEL_COUNT)
            };

            List<TelemVar> TruckData = new List<TelemVar>() {
                new ASCIITelemVar("ct_brand_id", "", "", "", 927),
                new ASCIITelemVar("ct_brand", "", "", "", 991),
                new ASCIITelemVar("ct_id", "", "", "", 1055),
                new ASCIITelemVar("ct_name", "", "", "", 1119),
                new FloatTelemVar("ct_fuel_capacity", "", "", "", 1119),
                new FloatTelemVar("ct_fuel_warning_factor", "", "", "", 1183),
                new FloatTelemVar("ct_adblue_capacity", "", "", "", 1187),
                new FloatTelemVar("ct_adblue_warning_factor", "", "", "", 1195),
                new FloatTelemVar("ct_air_pressure_warning", "", "", "", 1199),
                new FloatTelemVar("ct_air_pressure_emergency", "", "", "", 1203),
                new FloatTelemVar("ct_oil_pressure_warning", "", "", "", 1207),
                new FloatTelemVar("ct_water_temperature_warning", "", "", "", 1211),
                new FloatTelemVar("ct_battery_voltage_warning", "", "", "", 1215),
                new FloatTelemVar("ct_rpm_limit", "", "", "", 1219),
                new U32TelemVar("ct_forward_gear_count", "", "", "", 1223),
                new U32TelemVar("ct_reverse_gear_count", "", "", "", 1227),
                new FloatTelemVar("ct_differential_ratio", "", "", "", 1231),
                new U32TelemVar("ct_retarder_step_count", "", "", "", 1235),
                new FVectorTelemVar("ct_cabin_position", "", "", "", 1239),
                new FVectorTelemVar("ct_head_position", "", "", "", 1251),
                new FVectorTelemVar("ct_hook_position", "", "", "", 1263),
                new U32TelemVar("ct_wheel_count", "", "", "", 1275),
                new FVectorTelemVar("ct_wheel_position", "", "", "", 1279, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_steerable", "", "", "", 1375, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_simulated", "", "", "", 1383, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("ct_wheel_radius", "", "", "", 1391, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_powered", "", "", "", 1423, TRUCK_WHEEL_COUNT),
                new BoolTelemVar("ct_wheel_liftable", "", "", "", 1431, TRUCK_WHEEL_COUNT),
                new FloatTelemVar("ct_forward_ratio", "", "", "", 1439, FWD_GEAR_COUNT),
                new FloatTelemVar("ct_reverse_ratio", "", "", "", 1567, RVS_GEAR_COUNT)
            };

            List<TelemVar> TrailerData = new List<TelemVar>() {
                new ASCIITelemVar("cr_id", "", "", "", 1631),
                new ASCIITelemVar("cr_cargo_accessory_id", "", "", "", 1695),
                new FVectorTelemVar("cr_hook_position", "", "", "", 1759),
                new U32TelemVar("cr_wheel_count", "", "", "", 1771),
                new FVectorTelemVar("cr_wheel_position", "", "", "", 1775, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_steerable", "", "", "", 1967, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_simulated", "", "", "", 1983, TRAILER_WHEEL_COUNT),
                new FloatTelemVar("cr_wheel_radius", "", "", "", 1999, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_powered", "", "", "", 2063, TRAILER_WHEEL_COUNT),
                new BoolTelemVar("cr_wheel_liftable", "", "", "", 2079, TRAILER_WHEEL_COUNT)
            };

            List<TelemVar> JobData = new List<TelemVar>() {
                new ASCIITelemVar("cj_cargo_id", "", "", "", 2095),
                new ASCIITelemVar("cj_cargo", "", "", "", 2159),
                new FloatTelemVar("cj_cargo_mass", "", "", "", 2223),
                new ASCIITelemVar("cj_destination_city_id", "", "", "", 2227),
                new ASCIITelemVar("cj_destination_city", "", "", "", 2291),
                new ASCIITelemVar("cj_destination_company_id", "", "", "", 2355),
                new ASCIITelemVar("cj_destination_company", "", "", "", 2419),
                new ASCIITelemVar("cj_source_city_id", "", "", "", 2483),
                new ASCIITelemVar("cj_source_city", "", "", "", 2547),
                new ASCIITelemVar("cj_source_company_id", "", "", "", 2611),
                new ASCIITelemVar("cj_source_company", "", "", "", 2675),
                new U64TelemVar("cj_income", "", "", "", 2739),
                new U32TelemVar("cj_delivery_time", "", "", "", 2747)
            };

            U8TelemVar truckDataMarker = new U8TelemVar(null, null, null, null, 924);
            U8TelemVar trailerDataMarker = new U8TelemVar(null, null, null, null, 925);
            U8TelemVar jobDataMarker = new U8TelemVar(null, null, null, null, 926);

            EventVars = new EventVariableList(TruckData, TrailerData, JobData, truckDataMarker, trailerDataMarker, jobDataMarker);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string sender) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sender));
        }
    }
}