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
            .ForMember(dest => dest.Registrations, opt => opt.Ignore());

        CreateMap<UpdateStudentDto, Student>()
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Registrations, opt => opt.Ignore());

        // Course mappings
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.CurrentEnrollment, opt => opt.MapFrom(src => src.CurrentEnrollment));

        CreateMap<CreateCourseDto, Course>()
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Registrations, opt => opt.Ignore());

        CreateMap<UpdateCourseDto, Course>()
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Registrations, opt => opt.Ignore());

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
    }
}