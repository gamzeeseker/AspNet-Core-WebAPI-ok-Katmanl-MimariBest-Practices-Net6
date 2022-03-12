using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nlayer.Core
{
    public abstract class BaseEntity // new ile yeni bir nesne örneği oluşturamayız diye abstract yaptık
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } // bunun ilk kayıtta null m-olması gerek o yüzden ? yaptık, güncellemede bunu ekleriz
    }

    // genelde abstract class larımız projelerimizde ortak yani class larımızda ortak olan property ya da metodlarımızı tanımladığımız yerlerdir. 
    // interfacelerde soyut yapılardır onlarda da contract yani sözleşmelerimizi tanımlarız
    // interface ile yapabildiklerimizi abstract ile de yapabiliriz, ikisi de soyut yapılardır
}
