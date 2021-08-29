using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkovPriceViewer
{
    public class Ballistic
    {
        public String Name;
        public String Damage;
        public String PP;//Penetration_power
        public String AD;//Armor_damage
        public String Accuracy;
        public String Recoil;
        public String FC;//Fragmentation_chance
        public String BL;//Bleed_L
        public String BH;//Bleed_H
        //Bullet_effectiveness_against_armor_class
        public String BE1;
        public String BE2;
        public String BE3;
        public String BE4;
        public String BE5;
        public String BE6;

        public String Special;//subsonic or tracer

        public List<Ballistic> Calibarlist;

        public Ballistic(string name, string damage, string pP, string aD,
            string accuracy, string recoil, string fC, string bL, string bH,
            string bE1, string bE2, string bE3, string bE4, string bE5, string bE6, string special, List<Ballistic> calibarlist)
        {
            Name = name;
            Damage = damage;
            PP = pP;
            AD = aD;
            Accuracy = accuracy;
            Recoil = recoil;
            FC = fC;
            BL = bL;
            BH = bH;
            BE1 = bE1;
            BE2 = bE2;
            BE3 = bE3;
            BE4 = bE4;
            BE5 = bE5;
            BE6 = bE6;
            Special = special;
            Calibarlist = calibarlist;
        }

        public String[] Data()
        {
            return (new String[] {
                Name
                , Damage
                , BE1
                , BE2
                , BE3
                , BE4
                , BE5
                , BE6
            });
        }
    }
}
