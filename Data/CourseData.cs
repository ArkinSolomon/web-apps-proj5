using WebAppsProject5.Models;

namespace WebAppsProject5.Data;

public class CourseData(string id, string name, string description, float credits)
{
    public string Id { get; init; } = id;
    public string Name { get; init; } = name;
    public string Description { get; init; } = description;
    public float Credits { get; init; } = credits;
 
    public static CourseData FromCourse(Course course)
    {
        return new CourseData(course.Id, course.Name, course.Description, course.Credits);
    }
}