namespace Moshrefy.Web.Models.Statistics
{
    public class SystemStatisticsVM
    {
        // Centers
        public int TotalCenters { get; set; }
        public int ActiveCenters { get; set; }
        public int InactiveCenters { get; set; }
        
        // Users
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        
        // Teachers
        public int TotalTeachers { get; set; }
        public int DeletedTeachers { get; set; }
        
        // Students
        public int TotalStudents { get; set; }
        public int DeletedStudents { get; set; }
        
        // Courses
        public int TotalCourses { get; set; }
        public int DeletedCourses { get; set; }
        
        // Classrooms
        public int TotalClassrooms { get; set; }
        public int DeletedClassrooms { get; set; }
    }
}