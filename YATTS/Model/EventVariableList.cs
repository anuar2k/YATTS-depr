using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.MemoryMappedFiles;

namespace YATTS {
    public class EventVariableList {
        public ObservableCollection<TelemVar> DisplayedVars { get; private set; }
        public List<TelemVar> TruckEventVars { get; private set; }
        public List<TelemVar> TrailerEventVars { get; private set; }
        public List<TelemVar> JobEventVars { get; private set; }

        private U8TelemVar truckDataMarker;
        private byte lastTruckVal = 0;

        private U8TelemVar trailerDataMarker;
        private byte lastTrailerVal = 0;

        private U8TelemVar jobDataMarker;
        private byte lastJobVal = 0;

        public EventVariableList(List<TelemVar> TruckEventVars,
                                 List<TelemVar> TrailerEventVars,
                                 List<TelemVar> JobEventVars,
                                 U8TelemVar truckDataMarker,
                                 U8TelemVar trailerDataMarker,
                                 U8TelemVar jobDataMarker) {
            this.TruckEventVars = TruckEventVars;
            this.TrailerEventVars = TrailerEventVars;
            this.JobEventVars = JobEventVars;
            this.truckDataMarker = truckDataMarker;
            this.trailerDataMarker = trailerDataMarker;
            this.jobDataMarker = jobDataMarker;

            var tempDisplayedVars = new ObservableCollection<TelemVar>();

            TruckEventVars.ForEach((x) => tempDisplayedVars.Add(x));
            TrailerEventVars.ForEach((x) => tempDisplayedVars.Add(x));
            JobEventVars.ForEach((x) => tempDisplayedVars.Add(x));

            DisplayedVars = tempDisplayedVars;
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
