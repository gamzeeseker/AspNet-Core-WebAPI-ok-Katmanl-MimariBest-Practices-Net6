﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nlayer.Core
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}

// Entityler içerisinde farklı entity ve classlara refernas verdiğimiz property lere navigation property diyoruz
// Biz Category den Poduct lara erişebiliyoruz 
