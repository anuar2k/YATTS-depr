using System.Collections.Generic;
using System.IO.MemoryMappedFiles;

namespace YATTS {
    public class EventVariableList
    {
        public List<TelemVar> TruckData;
        public List<TelemVar> TrailerData;
        public List<TelemVar> JobData;

        private U8TelemVar truckDataMarker;
        private byte lastTruckVal = 0;

        private U8TelemVar trailerDataMarker;
        private byte lastTrailerVal = 0;

        private U8TelemVar jobDataMarker;
        private byte lastJobVal = 0;

        public EventVariableList(List<TelemVar> TruckData, 
                                List<TelemVar> TrailerData, 
                                List<TelemVar> JobData,
                                U8TelemVar truckDataMarker,
                                U8TelemVar trailerDataMarker,
                                U8TelemVar jobDataMarker) {
            this.TruckData = TruckData;
            this.TrailerData = TrailerData;
            this.JobData = JobData;
            this.truckDataMarker = truckDataMarker;
            this.trailerDataMarker = trailerDataMarker;
            this.jobDataMarker = jobDataMarker;
        }

        private DataStatus NewDataAvail(U8TelemVar marker, MemoryMappedViewAccessor source, ref byte lastMarkerValue) {
            byte newValue = marker.GetByteValue(source)[0];
            if (newValue != lastMarkerValue) {
                lastMarkerValue = newValue;
                if (newValue == 0) {
                    return DataStatus.CLEARED;
                } else {
                    return DataStatus.UPDATED;
                }
            } else {
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
}
