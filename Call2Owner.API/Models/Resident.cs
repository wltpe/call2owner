using System;
using System.Collections.Generic;

namespace Oversight.Model
{
    public class Resident
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SocietyFlatId { get; set; }
        public int? EntityTypeDetailId { get; set; }
        public bool IsDocumentUploaded { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string ApprovedComment { get; set; }
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
