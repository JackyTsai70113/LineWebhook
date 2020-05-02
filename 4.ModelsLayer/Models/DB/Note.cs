using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.DB {

    public class Note {

        [Key]
        public Guid Id { get; set; }

        public string Remark { get; set; }
    }
}