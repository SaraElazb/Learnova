UPDATE Courses
SET Course_Name = Title
WHERE Course_Name IS NULL OR Course_Name = ''; 