using System.Collections.ObjectModel;
using System.IO.MemoryMappedFiles;

namespace YATTS {
    public class EventVariableList {
        public ObservableCollection<TelemVar> EventVars { get; set; }

        private U8TelemVar truckDataMarker;
        private byte lastTruckVal = 0;

        private U8TelemVar trailerDataMarker;
        private byte lastTrailerVal = 0;

        private U8TelemVar jobDataMarker;
        private byte lastJobVal = 0;

        public EventVariableList(ObservableCollection<TelemVar> EventVars,
                                 U8TelemVar truckDataMarker,
                                 U8TelemVar trailerDataMarker,
                                 U8TelemVar jobDataMarker) {
            this.EventVars = EventVars;
            this.truckDataMarker = truckDataMarker;
            this.trailerDataMarker = trailerDataMarker;
            this.jobDataMarker = jobDataMarker;
        }

        private DataStatus NewDataAvail(U8TelemVar marker, MemoryMappedViewAccessor source, ref byte lastMarkerValue) {
            byte newValue = marker.GetByteValue(source, false)[0];
            if (newValue != lastMarkerValue) {
                lastMarkerValue = newValue;
                if (newValue == 0) {
                    return DataStatus.CLEARED;
                }
                else {
                    return DataStatus.UPDATED;
                }
            }
            else {
                return DataStatus.NO_NEW;
            }
        }

        public DataStatus NewTruckDataAvail(MemoryMappedViewAccessor source) {
            return NewDataAvail(truckDataMarker, source, ref lastTruckVal);
        }

        public DataStatus NewTrailerDataAvail(MemoryMappedViewAccessor source) {
            return NewDataAvail(trailerDataMarker, source, ref lastTrailerVal);
        }

        public DataStatus NewJobDataAvail(MemoryMappedViewAccessor source) {
            return NewDataAvail(jobDataMarker, source, ref lastJobVal);
        }
    }

    public enum DataStatus {
        NO_NEW,
        UPDATED,
        CLEARED
    }
}
