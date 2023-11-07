using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatLng {
    public double Latitude;
    public double Longitude;
    public string LatLngString;

    public LatLng(string spatial) {
        string[] split = spatial.Split( "," );
        double lat = 0f;
        double lng = 0f;
        if( double.TryParse( split[0], out lat ) && double.TryParse( split[1], out lng ) ) {
            Latitude = lat;
            Longitude = lng;
        }
        LatLngString = spatial;
    }

    public LatLng( double lat, double lng ) {
        Latitude = lat;
        Longitude = lng;
        LatLngString = $"{lat},{lng}";
    }

    //public override bool Equals( object obj ) {
    //    if(obj == null ) return false;
    //    if( obj is string s ) {
    //        string[] split = s.Split( "," );
    //        double lat = 0f;
    //        double lng = 0f;
    //        if( double.TryParse( split[0], out lat ) && double.TryParse( split[1], out lng ) ) {
    //            return lat == Latitude && lng == Longitude;
    //        }
    //        return false;
    //    }
    //    return base.Equals( obj );
    //}

    public override string ToString() {
        return $"{Latitude},{Longitude}";
    }
}
