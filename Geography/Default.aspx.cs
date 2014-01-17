// Created By: Brij Mohan
// For more information, visit http://techbrij.com


using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       // GenerateData();

    }
    /// <summary>
    /// To add random data in database for testing
    /// </summary>
    void GenerateData()
    {
        Random random = new Random();
        double lat = 25.3294781, lng = 55.373236899999995;
        using (var context = new SampleDBEntities())
        {

            for (int i = 1; i < 100; i++)
            {
                context.PlaceInfoes.Add(new PlaceInfo()
                {
                    Name = "Sample" + i,
                    Address = "address" + i,
                    City = "test city",
                    State = "test state",
                    CountryId = Convert.ToInt32((i+random.Next(20))%2),
                    Geolocation = DbGeography.FromText("POINT( " + (lng + random.NextDouble() * 1.55).ToString() + " " + (lat + random.NextDouble()).ToString() + ")")
                });
            }
            context.SaveChanges();
        }
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
         
        var currentLocation = DbGeography.FromText("POINT(" + hdnLocation.Value + ")"); 
        string[] latlng = hdnLocation.Value.Split(new char[]{' '}); 
        using ( var context = new SampleDBEntities()){

            var places = (from u in context.PlaceInfoes
                          orderby u.Geolocation.Distance(currentLocation)
                          select u).Take(5).Select(x => new { Name = x.Name, Address = x.Address, City = x.City, State = x.State, Latitude = x.Geolocation.Latitude, Longitude = x.Geolocation.Longitude });

            //Bind GridView
            GridView1.DataSource = places.ToList();
            GridView1.DataBind();

            //Set points for map
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(places);
            ClientScript.RegisterClientScriptBlock(GetType(), "points", "var points = " + output + ";var currentLoc = { 'Latitude' : " + latlng[1] + ", 'Longitude':"+latlng[0] +" }", true);
        }


    }
}