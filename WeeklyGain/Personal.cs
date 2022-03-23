using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WeeklyGain
{
    public class Profile
    {
        public string name { get; set; }

        public int created { get; set; }
    }
    public class Hero
    {
        public string id{ get; set; }
        public int rarity{ get; set; }
        public string mainClass { get; set; }
        public int staminaFullAt { get; set; }
        public string profession { get; set; }
    }
    public class lstHeroes
    {
        public List<Hero> Heroes { get; set; }
    }
    internal class PersonalData
    {
        public lstHeroes heroes = null;
        public Profile PersonalProfile = null;
        async Task<dynamic> GetHeroesString(string wallet)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestUri = string.Format("https://defi-kingdoms-community-api-gateway-co06z8vi.uc.gateway.dev/graphql?query=%7Bheroes(orderBy%3A%20numberId%20orderDirection%3A%20asc%20first%3A%201000%20where%3A%7Bowner%3A%22{0}%22%7D)%7Bid%20%2C%20rarity%20%2C%20mainClass%20%2C%20staminaFullAt%2Cprofession%7D%7D", wallet);
                HttpResponseMessage response = client.GetAsync(requestUri).Result;
                if (response.IsSuccessStatusCode)
                {
                    var contents = await response.Content.ReadAsStringAsync();
                    return contents;
                }
                return null;
            }
        }
        async Task<dynamic> GetProfileString(string wallet)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestUri = string.Format("https://defi-kingdoms-community-api-gateway-co06z8vi.uc.gateway.dev/graphql?query=%7Bprofile%20(id%3A%22{0}%22)%20%7Bname%2Ccreated%7D%7D", wallet);
                HttpResponseMessage response = client.GetAsync(requestUri).Result;
                if (response.IsSuccessStatusCode)
                {
                    var contents = await response.Content.ReadAsStringAsync();
                    return contents;
                }
                return null;
            }
        }
        public void LoadHeroes(string wallet)
        {
        var t = Task.Run(() => GetHeroesString(wallet));
        t.Wait();

            if (t != null)
            {
                dynamic stuff = JObject.Parse((t.Result));
                try
                {
                    heroes = stuff["data"].ToObject<lstHeroes>();
                }
                catch
                {
                    heroes = null;
                }
            }
        
            t = Task.Run(() => GetProfileString(wallet));
            t.Wait();

            if (t != null)
            {
                dynamic stuff = JObject.Parse((t.Result));
                try
                {
                    PersonalProfile = stuff["data"]["profile"].ToObject<Profile>();
                }
                catch
                {
                    PersonalProfile = new Profile();
                    PersonalProfile.name = stuff["data"]["profile"]["name"];
                    PersonalProfile.created = 0;
                }
            }
        }

        public static string GetRarityString(int rarity)
        {
            if (rarity == 0) return "Common";
            if (rarity == 1) return "Uncommon";
            if (rarity == 2) return "Rare";
            if (rarity == 3) return "Legendary";
            if (rarity == 4) return "Mythic";
            return "";
        }
    }
}
