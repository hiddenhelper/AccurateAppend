// This broke when I changed the Record format. It doesnt appear that this comparitor is used, so I'm not going to bother fixing it until I find out otherwise.

//using System.Collections.Generic;
//using ListBuilder.Models;

//namespace ListBuilder.DataSources.ConsumerProfile
//{
//    public class RecordComparer : IEqualityComparer<Record>
//    {
//        public bool Equals(Record x, Record y)
//        {
//            if (x.FirstName.ToLower() == y.FirstName.ToLower()
//                && (x.LastName.ToLower() == y.LastName.ToLower())
//                && (x.Address.HouseNumber.ToLower() == y.Address.HouseNumber.ToLower())
//                && (x.Address.StreetName.ToLower() == y.Address.StreetName.ToLower())
//                && (x.Address.City.Name.ToLower() == y.Address.City.Name.ToLower())
//                && (x.Address.Zip.Name.ToLower() == y.Address.Zip.Name.ToLower())
//                && (x.Dob == y.Dob)
//                )
//                return true;

//            return false;
//        }
//        public int GetHashCode(Record a)
//        {
//            var z = a.FirstName.ToLower() 
//                + a.LastName.ToLower() 
//                + a.Address.StreetAddress.ToLower() 
//                + a.Address.City.Name.ToLower() 
//                + a.Address.HouseNumber.ToLower() 
//                + a.Address.StreetName.ToLower() 
//                + a.Address.Zip.Name.ToLower() 
//                + a.Dob;
//            return (z.GetHashCode());
//        }
//    }
//}
