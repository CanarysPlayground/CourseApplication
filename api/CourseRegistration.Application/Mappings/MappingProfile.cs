using AutoMapper;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Domain.Entities;

namespace CourseRegistration.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Student mappings
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age));

        CreateMap<CreateStudentDto, Student>()
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Registrations, opt => opt.Ignore())
            .ForMember(dest => dest.WaitlistEntries, opt => opt.Ignore());

        CreateMap<UpdateStudentDto, Student>()
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Registrations, opt => opt.Ignore())
            .ForMember(dest => dest.WaitlistEntries, opt => opt.Ignore());

        // Course mappings
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.CurrentEnrollment, opt => opt.MapFrom(src => src.CurrentEnrollment))
            .ForMember(dest => dest.IsFull, opt => opt.MapFrom(src => src.IsFull))
            .ForMember(dest => dest.WaitlistCount, opt => opt.MapFrom(src => src.WaitlistEntries.Count(w => w.IsActive)));

        CreateMap<CreateCourseDto, Course>()
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Registrations, opt => opt.Ignore())
            .ForMember(dest => dest.WaitlistEntries, opt => opt.Ignore());

        CreateMap<UpdateCourseDto, Course>()
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Registrations, opt => opt.Ignore())
            .ForMember(dest => dest.WaitlistEntries, opt => opt.Ignore());

        // Registration mappings
        CreateMap<Registration, RegistrationDto>()
            .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.Student))
            .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course));

        CreateMap<CreateRegistrationDto, Registration>()
            .ForMember(dest => dest.RegistrationId, opt => opt.Ignore())
            .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Grade, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());

        // Mapping for updating registration status
        CreateMap<UpdateRegistrationStatusDto, Registration>()
            .ForMember(dest => dest.RegistrationId, opt => opt.Ignore())
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());

        // Waitlist mappings
        CreateMap<WaitlistEntry, WaitlistEntryDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName))
            .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.Email))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.CourseName));

        CreateMap<CreateWaitlistEntryDto, WaitlistEntry>()
            .ForMember(dest => dest.WaitlistEntryId, opt => opt.Ignore())
            .ForMember(dest => dest.JoinedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Position, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.NotifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());

        CreateMap<UpdateWaitlistEntryDto, WaitlistEntry>()
            .ForMember(dest => dest.WaitlistEntryId, opt => opt.Ignore())
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.JoinedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.NotifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}