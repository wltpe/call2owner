using System;
using System.Collections.Generic;

namespace Oversight.Model
{
    public class SocietyDocumentUploaded
    {
        public int Id { get; set; }
        public int SocietyId { get; set; }
        public int SocietyDocumentRequiredToRegisterId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
