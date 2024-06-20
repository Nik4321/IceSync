namespace IceSync.Models
{
    public class WorkflowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string MultiExecBehavior { get; set; }
    }
}
