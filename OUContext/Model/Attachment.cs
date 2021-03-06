﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
   
    public class Attachment
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string MasterType { get; set; }
        public int MasterId { get; set; }
        [Required]
        [MaxLength(100)]
        [DisplayName("文档名")]
        public string FileName { get; set; }
        [DisplayName("文档类型")]
        public string ContentType { get; set; }
        [DisplayName("文档大小")]
        public int Length { get; set; }
        [Required]
       
        public byte[] Contents { get; set; }
    }
}
