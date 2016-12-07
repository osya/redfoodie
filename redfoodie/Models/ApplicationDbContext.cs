using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace redfoodie.Models
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        private static readonly Random Rnd = new Random();

        private static DbGeography CreatePoint(double latitude, double longitude)
        {
            var point = string.Format(CultureInfo.InvariantCulture.NumberFormat, "POINT({0} {1})", longitude, latitude);
            // 4326 is most common coordinate system used by GPS/Maps
            return DbGeography.PointFromText(point, 4326);
        }

        private static string GetStringId(string value)
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            return Regex.Replace(textInfo.ToTitleCase(value), "[ -&.()]", "");
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var cuisineList = new[]
            {
                "Afghani", "African", "American", "Arabian", "Asian", "Assamese", "Bakery", "Bengali", "Beverages",
                "Biryani", "British", "Burmese", "Cafe", "Cantonese", "Chettinad", "Chinese", "Coffee & tea",
                "Contemporary", "Continental", "Desserts", "European", "Fast food", "Finger food", "French", "German",
                "Goan", "Greek", "Gujarati", "Healthy food", "Hyderabadi", "Ice cream", "Indian", "Indonesian",
                "International", "Iranian", "Italian", "Japanese", "Juices", "Kashmiri", "Kerala", "Konkan", "Korean",
                "Lebanese", "Lounge", "Lucknowi", "Maharashtrian", "Malaysian", "Malwani", "Mangalorean",
                "Mediterranean", "Mexican", "Middle eastern", "Moroccan", "Mughlai", "Multi cuisine", "Naga", "Nepalese",
                "North indian", "Oriya", "Others", "Pakistani", "Parsi", "Persian", "Pizza", "Portuguese", "Pub",
                "Rajasthani", "Raw meats", "Sea food", "Sichuan", "Singaporean", "South indian", "Spanish",
                "Steak house", "Street food", "Tex - mex", "Thai", "Tibetan", "Turkish", "Vietnamese"
            };
            
            foreach (var cuisine in cuisineList)
            {
                context.Cuisines.Add(new Cuisine { Id = GetStringId(cuisine), Name = cuisine });
            }
            context.SaveChanges();

            var cities = new Dictionary<string, string[]>
            {
                {"Delhi NCR", new[]
                    {
                        "Alaknanda", "Ambience mall", "Anand lok", "Anand niketan", "Anand vihar", "Ardee city",
                        "Ashok vihar phase 1", "Ashok vihar phase 2", "Ashok vihar phase 3", "Aurangzeb road", "Aurobindo marg",
                        "Azad market", "Azadpur", "Badarpur border", "Ballabhgarh", "Barakhamba road", "Bhikaji cama place", "Chanakyapuri",
                        "Chander nagar", "Chandni chowk", "Charmwood village", "Chawri bazar", "Chhatarpur", "Chiranjeev vihar",
                        "Chittaranjan park", "Civil lines", "Connaught place", "Crossing republik", "Daryaganj",
                        "Defence colony", "Delhi cantt.", "Delhi university - gt...", "Dilshad garden", "Dlf cyber city",
                        "Dlf phase 1", "Dlf phase 2", "Dlf phase 3", "Dlf phase 4", "Dlf phase 5", "Dr. zakir hussain mar...", "Dwarka",
                        "East of kailash", "East patel nagar", "Friends colony", "Geeta colony", "Geetanjali enclave",
                        "Gole market", "Golf course road", "Govind puram", "Greater kailash (gk)...", "Greater noida", "Green park", "Gtb nagar", "Gujranwala town", "Hauz khas",
                        "Hauz khas village", "Igi airport", "Inderlok", "India gate", "Indirapuram", "Indraprastha",
                        "Indraprastha colony", "Ip extension", "Jail road", "Jama masjid", "Janakpuri", "Jangpura", "Janpath",
                        "Jasola", "Jnu", "Jor bagh", "Kailash colony", "Kalkaji", "Kamla nagar", "Kapashera", "Karampura",
                        "Karkardooma", "Karol bagh", "Kashmiri gate", "Kaushambi", "Kavi nagar", "Khan market", "Khanpur",
                        "Khel gaon marg", "Kirti nagar", "Krishna nagar", "Lajpat nagar 1", "Lajpat nagar 2", "Lajpat nagar 3",
                        "Lajpat nagar 4", "Lawrence road", "Laxmi nagar", "Lodhi colony", "Lodhi road", "Loni", "Mahipalpur",
                        "Majnu ka tila", "Malcha marg", "Malviya nagar", "Mandi house", "Mansingh road", "Mathura road",
                        "Mayapuri phase 1", "Mayapuri phase 2", "Mayur vihar phase 1", "Mayur vihar phase 2",
                        "Mayur vihar phase 3", "Mehrauli", "Mg road", "Model town 1", "Model town 2", "Model town 3",
                        "Mohan nagar", "Moti bagh", "Moti nagar", "Mukherjee nagar", "Munirka", "Najafgarh", "Nangloi",
                        "Naraina", "Narela", "Nehru nagar", "Nehru place", "Netaji nagar", "Netaji subhash place",
                        "New friends colony", "Nit", "Nizamuddin", "Okhla phase 1", "Okhla phase 2", "Okhla phase 3",
                        "Old railway road", "Paharganj", "Palam", "Palam vihar", "Panchsheel park", "Pandara road market",
                        "Pandav nagar", "Paschim vihar", "Patparganj", "Pitampura", "Pragati maidan", "Prashant vihar",
                        "Pratap vihar", "Preet vihar", "Punjabi bagh", "Qutab institutional a...", "R k puram", "Race course",
                        "Raj nagar", "Raja garden", "Rajendar nagar", "Rajendar place", "Rajinder nagar", "Rajokri",
                        "Rajouri garden", "Ramprastha", "Rana pratap bagh", "Rohini", "Sadar bazar", "Safdarjung", "Sahibabad",
                        "Sainik colony", "Sainik farms", "Saket", "Sardar patel marg", "Sarita vihar", "Sarojini nagar", "Satyaniketan",
                        "Sda", "Sector 1", "Sector 10", "Sector 11", "Sector 110", "Sector 12",
                        "Sector 125", "Sector 127", "Sector 132", "Sector 14",
                        "Sector 15", "Sector 16", "Sector 17", "Sector 18", "Sector 19",
                        "Sector 2", "Sector 20", "Sector 21", "Sector 22", "Sector 23",
                        "Sector 25", "Sector 26", "Sector 27", "Sector 28", "Sector 29",
                        "Sector 3", "Sector 30", "Sector 31", "Sector 32", "Sector 33",
                        "Sector 34", "Sector 35", "Sector 37", "Sector 38", "Sector 39",
                        "Sector 4", "Sector 40", "Sector 41", "Sector 42", "Sector 43", "Sector 44",
                        "Sector 45", "Sector 46", "Sector 48", "Sector 49", "Sector 5",
                        "Sector 50", "Sector 51", "Sector 52", "Sector 53", "Sector 54", "Sector 55",
                        "Sector 56", "Sector 57", "Sector 58", "Sector 59", "Sector 6", "Sector 60", "Sector 61",
                        "Sector 62", "Sector 63", "Sector 65", "Sector 7", "Sector 71", "Sector 72",
                        "Sector 8", "Sector 81", "Sector 82", "Sector 83", "gurgaon",
                        "Sector 86", "Sector 9", "Sector 92", "Sector 93", "Shahdara", "Shahpur jat", "Shakarpur",
                        "Shalimar bagh", "Shalimar garden", "Shiekh sarai", "Sikandrapur", "Siri fort road", "Sohna road",
                        "South city 1", "South city 2", "South extension 1", "South extension 2", "South patel nagar",
                        "Subhash nagar", "Sundar nagar", "Suraj kund", "Surajpur", "Sushant lok", "Tagore garden",
                        "Taj express highway", "Tilak marg", "Tilak nagar", "Tughlakabad instituti...", "Uday park",
                        "Udyog vihar", "Uttam nagar", "Vaishali", "Vasant kunj", "Vasant vihar", "Vasundhara",
                        "Vasundhara enclave", "Vijay nagar", "Vikas marg", "Vikaspuri", "Vivek vihar", "Wazirabad", "Wazirpur",
                        "West patel nagar", "Yusuf sarai", "Zakir nagar"
                    }},
                { "Amritsar", new[] { "Basant nagar", "Gt road", "Ina colony", "Kabir park", "Lawrence road", "Mall road", "Rani ka bagh", "Ranjit avenue", "Shastri nagar", "Town hall", "White avenue" }},
                { "Chandigarh", new[] {"Chandigarh industrial...", "It park", "Kharar road", "Manimajra", "Phase 1", "Phase 10", "Phase 11", "Phase 2", "Phase 3", "Phase 5", "Phase 6", "Phase 7", "Phase 9", "Sector 10", "Sector 10", "Sector 11", "Sector 11", "Sector 12", "Sector 14", "Sector 14", "Sector 15", "Sector 15", "Sector 16", "Sector 16", "Sector 17", "Sector 18", "Sector 19", "Sector 2", "Sector 20", "Sector 21", "Sector 22", "Sector 23", "Sector 24", "Sector 26", "Sector 27", "Sector 28", "Sector 30", "Sector 32", "Sector 33", "Sector 34", "Sector 35", "Sector 36", "Sector 37", "Sector 38", "Sector 40", "Sector 42", "Sector 43", "Sector 44", "Sector 46", "Sector 47", "Sector 5", "Sector 6", "Sector 67", "Sector 7", "Sector 7", "Sector 70", "Sector 71", "Sector 8", "Sector 8", "Sector 9", "Sector 9", "Zirakpur"}},
                { "Jaipur", new[] {"Adarsh nagar", "Agra road", "Ajmer highway", "Amber", "Bais godam", "Bani park", "Bapu nagar", "Brahmpuri", "C scheme", "Chitrakoot", "Civil lines", "Gopalbari", "Hasanpura", "Jawahar nagar", "Jhotwara", "Johari bazar", "Khatipura road", "Kukas", "Lal kothi", "Malviya nagar", "Mansarovar", "Mi road", "Narayan singh circle", "Pink city", "Raja park", "Sansar chandra road", "Shastri nagar", "Shyam nagar", "Sikar road", "Sindhi camp", "Sodala", "Tonk road", "Transport nagar", "Vaishali nagar", "Vidyadhar nagar"}},
                { "Ludhiana", new[] {"Aggar nagar", "Brs nagar", "Civil lines", "Dugri", "Gurdev nagar", "Hargobind nagar", "Industrial area", "Janak puri", "Jawahar nagar", "Ludhiana junction", "Model town", "Pakhowal road", "Pau", "Rajguru nagar", "Samrala chowk", "Sarabha nagar", "Sector 32", "Shastri nagar"}},
                { "Mumbai", new[] {"7 bungalows, andheri...", "Adarsh nagar, andheri...", "Airoli", "Andheri east", "Andheri lokhandwala,...", "Andheri west", "Bandra east", "Bandra kurla complex", "Bandra west", "Bandstand, bandra wes...", "Bhandup", "Bhayandar", "Borivali east", "Borivali west", "Breach candy", "Byculla", "Carter road, bandra w...", "Castle mill, thane we...", "Cbd-belapur", "Chandivali", "Charni road", "Chembur", "Chowpatty", "Churchgate", "Colaba", "Cuffe parade", "Dadar", "Dadar east", "Dadar shivaji park", "Dadar west", "Dahisar", "Dombivali", "Dombivali east", "Dombivali west", "Fort", "Ghansoli", "Ghatkopar east", "Ghatkopar west", "Ghodbunder road", "Girgaum", "Gorai", "Goregaon", "Goregaon east", "Goregaon west", "Grant road", "Hill road, bandra wes...", "Jogeshwari", "Juhu", "Kalbadevi", "Kalwa", "Kalyan", "Kandivali", "Kandivali east", "Kandivali west", "Kasarvadavli, thane w...", "Kemps corner", "Khar", "Kharghar", "Khopat", "Kopar khairane", "Kurla", "Linking road, bandra...", "Lower parel", "Mahakali", "Mahalaxmi", "Mahim", "Majiwada, thane west", "Malabar hill", "Malad east", "Malad west", "Manpada, thane west", "Marine lines", "Marol", "Matunga east", "Matunga west", "Mira road", "Mulund east", "Mulund west", "Mumbai central", "Mumbai cst area", "Mumbra", "Nariman point", "Naupada, thane west", "Nerul", "Oshiwara, andheri wes...", "Pali hill, bandra wes...", "Panch pakhadi", "Panch pakhadi, thane...", "Panvel", "Parel", "Peddar road", "Powai", "Prabhadevi", "Reclamation, bandra w...", "Sakinaka", "Sanpada", "Santacruz east", "Santacruz west", "Seawoods", "Sion", "Tardeo", "Thane area", "Thane east", "Turbhe", "Ulhasnagar", "Vasai", "Vasant vihar, thane w...", "Vashi", "Versova, andheri west", "Vikhroli", "Vile parle east", "Vile parle west", "Wadala", "Wagle estate, thane w...", "Worli"}},
                { "Pune", new[] {"Akurdi", "Aundh", "B.t. kawade road", "Balewadi", "Baner", "Bavdhan", "Bhawani peth", "Bhosari", "Bibvewadi", "Budhwar peth", "Bund garden road", "Camp area", "Chakan", "Chandan nagar", "Chinchwad", "Dange chowk", "Deccan gymkhana", "Dehu road", "Dhankawadi", "Dhanori", "Dhole patil road", "East street", "Erandwane", "Expressway", "Fatima nagar", "Fc road", "Hadapsar", "Hinjewadi", "J.m. road", "Kalyani nagar", "Karve nagar", "Katraj", "Khadakwasla", "Khadki", "Kharadi", "Kondhwa", "Koregaon park", "Kothrud", "Lavale", "Lavasa", "Law college road", "Lohegaon", "Lonavala", "Magarpatta", "Mg road", "Model colony", "Mukund nagar", "Mulshi", "Narhe", "Nibm road", "Nigdi", "Old mumbai-pune highw...", "Parvathi", "Pashan", "Pimple gurav", "Pimple nilakh", "Pimple saudagar", "Pimpri", "Pradhikaran", "Pune university", "Pune-solapur road", "Rasta peth", "Sadashiv peth", "Salunkhe vihar road", "Sassoon road", "Satara road", "Senapati bapat road", "Shaniwar peth", "Shivaji nagar", "Shivapur", "Shukrawar peth", "Sinhgad road", "Swargate", "Tilak road", "Viman nagar", "Vishrantwadi", "Wagholi", "Wakad", "Wanowrie", "Warje", "Yerawada"}} 
            };
            foreach (var city in cities)
            {
                var cityId = GetStringId(city.Key);
                context.Cities.Add(new City { Id = cityId, Name = city.Key });
                foreach (var place in city.Value)
                {
                    try
                    {
                        context.Places.Add(new Place {Name = place, CityId = cityId});
                    }
                    catch (Exception ex)
                    {
                        ;
                    }

                }
            }
            context.SaveChanges();

            var restaurants = new Dictionary<string, Dictionary<string, Restaurant[]>>
            {
                { "DelhiNCR", new Dictionary<string, Restaurant[]>
                {
                    {"Chanakyapuri", new[] {
                        new Restaurant {
                        Name = "Capital Kitchen - Taj Palace Hotel",
                        ImageFileName = "80x80_14925471_10157806793365085_6999532131779578556_n-capital-kitchen-taj-palace-hotel-chanakyapuri-dff0fe8246e5b1b39279454475cc1.jpg",
                        Location = CreatePoint(28.59196, 77.188775),
                        Cuisines = new[] { context.Cuisines.Find("Asian"), context.Cuisines.Find("European"), context.Cuisines.Find("NorthIndian") }
                    }  } },
                    { "Saket", new [] {new Restaurant {
                        Name = "Yum Yum Cha",
                        ImageFileName = "80x80_review-yum-yum-cha-saket-new-delhi-32fa6ed732ce57d247948973624bfccc.jpg",
                        Location = CreatePoint(28.528877090780124, 77.21914628051763),
                        Cuisines = new [] { context.Cuisines.Find("Chinese"), context.Cuisines.Find("Japanese") }
                    } } },
                    { "Dwarka", new [] {new Restaurant {
                        Name = "Biryani Blues",
                        ImageFileName = "80x80_11042949_611278912305865_7781689162808340120_n-biryani-blues-sector-11-67defe9453e5ac16b7cad1107155a399.jpg",
                        Location = CreatePoint(28.374221, 77.320361),
                        Cuisines = new [] { context.Cuisines.Find("Biryani"), context.Cuisines.Find("Hyderabadi") }
                    } } }
                } }
            };

            foreach (var item in restaurants)
            {
                var city = context.Cities.Find(item.Key);
                if (item.Value == null) continue;
                foreach (var place in item.Value)
                {
                    var placeEnt = context.Places.FirstOrDefault(p => string.Equals(p.Name, place.Key) && p.CityId == city.Id);
                    if (placeEnt == null) continue;
                    placeEnt.CityId = city.Id;
                    foreach (var restaurant in place.Value)
                    {
                        restaurant.PlaceId = placeEnt.Id;
                        context.Restaurants.Add(restaurant);
                    }
                }
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);
            userManager.Create(new ApplicationUser { UserName = "1@ya.ru", Email = "1@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1980, 1, 1), Verified = true }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Satinder", Email = "2@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1911, 1, 1), ImageFileName = "146x146_47d93a09346478c900d31e1920c57bd5c627d612.jpg", Verified = true }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Kalki", Email = "3@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1912, 1, 1), ImageFileName = "146x146_489b42243d3b25094806a8bad87d482981c2d519.jpg", Verified = true }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Suneha Prakash", Email = "4@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1969, 1, 1) }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "VforVictory", Email = "5@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1978, 1, 1), ImageFileName = "146x146_013977b1504db7471d0f2e16b9aff2a3cb47a50f.jpg" }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Raman", Email = "6@ya.ru", City = context.Cities.Local.First() }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Veggie Foodie", Email = "7@ya.ru", City = context.Cities.Local.First() }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Himani Gupta", Email = "8@ya.ru", City = context.Cities.Local.First() }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Gopal Duggal", Email = "9@ya.ru", City = context.Cities.Local.First() }, "Aa123#");
            
            foreach (var user in context.Users.Local)
            {
                foreach (var restaurant in context.Restaurants.Local)
                {
                    context.Votes.Add(new Vote { ApplicationUser = user, Restaurant = restaurant, Value = Rnd.NextDouble() >= 0.5 });
                }
            }
            context.SaveChanges();

            base.Seed(context);
        }
    }

    public sealed partial class ApplicationDbContext
    {
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
