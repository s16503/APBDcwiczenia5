


use s16503
/*
-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2020-03-22 07:12:20.376

-- tables
-- Table: Enrollment
CREATE TABLE Enrollment (
    IdEnrollment int  NOT NULL,
    Semester int  NOT NULL,
    IdStudy int  NOT NULL,
    StartDate date  NOT NULL,
    CONSTRAINT Enrollment_pk PRIMARY KEY  (IdEnrollment)
);

-- Table: Student
CREATE TABLE Student (
    IndexNumber nvarchar(100)  NOT NULL,
    FirstName nvarchar(100)  NOT NULL,
    LastName nvarchar(100)  NOT NULL,
    BirthDate date  NOT NULL,
    IdEnrollment int  NOT NULL,
    CONSTRAINT Student_pk PRIMARY KEY  (IndexNumber)
);

-- Table: Studies
CREATE TABLE Studies (
    IdStudy int  NOT NULL,
    Name nvarchar(100)  NOT NULL,
    CONSTRAINT Studies_pk PRIMARY KEY  (IdStudy)
);

-- foreign keys
-- Reference: Enrollment_Studies (table: Enrollment)
ALTER TABLE Enrollment ADD CONSTRAINT Enrollment_Studies
    FOREIGN KEY (IdStudy)
    REFERENCES Studies (IdStudy);

-- Reference: Student_Enrollment (table: Student)
ALTER TABLE Student ADD CONSTRAINT Student_Enrollment
    FOREIGN KEY (IdEnrollment)
    REFERENCES Enrollment (IdEnrollment);

-- End of file.


INSERT INTO Studies(IdStudy,Name)
VALUES (1,'Informatyka');

INSERT INTO Enrollment(IdEnrollment,Semester,  IdStudy, StartDate)
VALUES (23243,2,1,'2019-02-03');

INSERT INTO Student(IndexNumber,FirstName, lastname, birthdate, IdEnrollment)
VALUES (14233,'Jan','Kowalski','1999-02-03', 23243);

INSERT INTO Student(IndexNumber,FirstName, lastname, birthdate, IdEnrollment)
VALUES (15333,'Andrzej','Nowak','2000-02-03', 23243);

INSERT INTO Student(IndexNumber,FirstName, lastname, birthdate, IdEnrollment)
VALUES (16444,'Jan','Kowalski','1997-02-03', 23243);

SELECT * FROM student;
*/



SELECT Enrollment.IdEnrollment, Semester, Studies.Name, StartDate FROM Enrollment JOIN Student ON Student.IdEnrollment = Enrollment.IdEnrollment JOIN Studies ON Enrollment.IdStudy=Studies.IdStudy WHERE Student.IndexNumber = 14233;


DELETE FROM Enrollment WHERE Enrollment.IdStudy = 1;
DELETE Studies WHERE Studies.name = 'Informatyka';

    INSERT INTO Studies VALUES(3,'Architektura wnętrz');
    INSERT INTO Enrollment values(433,3,3,'2018-10-01');


    INSERT INTO Studies VALUES(1,'IT');
    INSERT INTO Enrollment values(2,2,1,'2020-02-01');


    INSERT INTO Student(IndexNumber,FirstName, lastname, birthdate, IdEnrollment)
        VALUES (12320,'Aleksander','Byk','1996-02-03', 433);



      SELECT Count(*) as isnt FROM Enrollment JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy WHERE Studies.Name = 'IT';

      SELECT IdEnrollment FROM Enrollment JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy WHERE Studies.Name = 'IT' AND Enrollment.Semester = 1;

      SELECT * FROM Enrollment;



      SELECT IdEnrollment FROM Enrollment WHERE IdStudy =1 AND Enrollment.Semester = 1 ORDER BY StartDate;

                    SELECT * FROM student;      
                    
                     SELECT * FROM studies;   

    DELETE FROM Student;

    Select * from student;


    INSERT INTO Student(IndexNumber, FirstName, LastName,BirthDate, IdEnrollment) 
  VALUES('s121212', 'Anna', 'Gold', CONVERT(DATETIME,'20.12.2020',104), 2 );

  SELECT Studies.IdStudy FROm Studies WHERE Studies.Name = ' ';


create procedure Promotions @Studies varchar(100),@Semester int
as
declare @idEnrollment Int,@idStudies int,@id int,@oldIdEnrollment int,@Info varchar(200),@newSemester int;
begin
set @newSemester=@Semester+1;
Select @idStudies = Studies.IdStudy from Studies where Name=@Studies;
Select @oldIdEnrollment = IdEnrollment from Enrollment where Semester=@Semester and IdStudy=@idStudies
Select @idEnrollment = Enrollment.IdEnrollment from Enrollment where Semester=@newSemester and IdStudy=@idStudies;
if @oldIdEnrollment is not null
begin
if @idEnrollment is null
begin
Select @id= Max(ISnull(IdEnrollment,0))+ 1 from Enrollment;
insert into Enrollment(IdEnrollment,Semester,IdStudy,StartDate)
values(@id,@newSemester,@idStudies,GETDATE());
set @Info='dodoano Enrolment';
Print @Info;
Update Student set IdEnrollment = @id where IdEnrollment=@oldIdEnrollment;
set @Info='zmieniono';
print @Info;
end;
else
--set @id=@idEnrollment;
Update Student set IdEnrollment = @idEnrollment where IdEnrollment=@oldIdEnrollment;
set @Info='zmieniono';
print @Info;
end;
else
set @Info='Nie istnieje';
print @Info;
commit;
end;


DELETE student WHERE IdEnrollment = 547;

SELECT * FROM Enrollment WHERE Semester = 5 AND IdStudy = 1 ORDER BY IdEnrollment;

SELECT count(*) as count FROM Student WHERE IndexNumber ='s1234';


SELECT * FROM Student;
SELECT * FROM Enrollment;

 INSERT INTO Student(IndexNumber,FirstName, lastname, birthdate, IdEnrollment)
        VALUES (12320,'Aleksander','Byk','1996-02-03', 433);