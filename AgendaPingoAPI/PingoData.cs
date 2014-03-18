using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AgendaPingoAPI
{
    [DataContract]
    public class PingoData
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public Data data { get; set; }
    }

    public class Data
    {
        public int unit_id { get; set; }
        public List<double> values { get; set; }
    }

    [DataContract]
    public class Datum
    {
        [DataMember]
        public int meter_id { get; set; }
        [DataMember]
        public string meter_name { get; set; }
        [DataMember]
        public string installation_name { get; set; }
        [DataMember]
        public string timezone { get; set; }
        [DataMember]
        public int utc_offset { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public List<Datum> data { get; set; }
    }
}
