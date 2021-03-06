//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Organizer.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class TodoItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TodoItem()
        {
            this.Tags = new HashSet<Tag>();
        }
    
        public int Id { get; set; }
        public string Description { get; set; }
        public System.DateTime AddedOn { get; set; }
        public System.DateTime Deadline { get; set; }
        public bool ResolvesActivity { get; set; }
        public int ActivityId { get; set; }
        public bool Resolved { get; set; }
        public int Duration { get; set; }
        public string Notes { get; set; }
    
        public virtual Activity Activity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
