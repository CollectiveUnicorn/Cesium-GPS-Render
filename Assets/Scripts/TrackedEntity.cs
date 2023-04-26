using System;
using SQLite4Unity3d;

[System.Serializable]
public class TrackedEntity {
	public int Id;
	public string UUID;
	public double Lat;
	public double Lon;
    public double Alt;
    public String Time;
    public String FirstName;
    public String LastName;
    public String CompanyName;
    public String PhoneNumber;

    public TrackedEntity() {}

    public TrackedEntity(int Id, string Uuid, double lat, double lon, double alt, String time, String firstName, String lastName, String companyName, String phoneNumber) {
        this.Id = Id;
        this.UUID = Uuid;
        this.Lat = lat;
        this.Lon = lon;
        this.Alt = alt;
        this.Time = time;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.CompanyName = companyName;
        this.PhoneNumber = phoneNumber;
    }
}