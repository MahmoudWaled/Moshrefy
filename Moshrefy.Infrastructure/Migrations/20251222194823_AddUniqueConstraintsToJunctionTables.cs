using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moshrefy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintsToJunctionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherItems_TeacherId_ItemId",
                table: "TeacherItems");

            migrationBuilder.DropIndex(
                name: "IX_TeacherCourses_TeacherId_CourseId",
                table: "TeacherCourses");

            migrationBuilder.DropIndex(
                name: "IX_ExamResults_StudentId",
                table: "ExamResults");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherItem_Unique_Teacher_Item_Center",
                table: "TeacherItems",
                columns: new[] { "TeacherId", "ItemId", "CenterId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCourse_Unique_Teacher_Course_Center",
                table: "TeacherCourses",
                columns: new[] { "TeacherId", "CourseId", "CenterId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResult_Unique_Student_Exam_Center",
                table: "ExamResults",
                columns: new[] { "StudentId", "ExamId", "CenterId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_Unique_Student_Course_Center",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId", "CenterId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_Unique_Student_Session_Center",
                table: "Attendances",
                columns: new[] { "StudentId", "SessionId", "CenterId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherItem_Unique_Teacher_Item_Center",
                table: "TeacherItems");

            migrationBuilder.DropIndex(
                name: "IX_TeacherCourse_Unique_Teacher_Course_Center",
                table: "TeacherCourses");

            migrationBuilder.DropIndex(
                name: "IX_ExamResult_Unique_Student_Exam_Center",
                table: "ExamResults");

            migrationBuilder.DropIndex(
                name: "IX_Enrollment_Unique_Student_Course_Center",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_Unique_Student_Session_Center",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherItems_TeacherId_ItemId",
                table: "TeacherItems",
                columns: new[] { "TeacherId", "ItemId" },
                unique: true,
                filter: "[TeacherId] IS NOT NULL AND [ItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCourses_TeacherId_CourseId",
                table: "TeacherCourses",
                columns: new[] { "TeacherId", "CourseId" },
                unique: true,
                filter: "[TeacherId] IS NOT NULL AND [CourseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_StudentId",
                table: "ExamResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseId",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId" },
                unique: true,
                filter: "[StudentId] IS NOT NULL AND [CourseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances",
                column: "StudentId");
        }
    }
}
