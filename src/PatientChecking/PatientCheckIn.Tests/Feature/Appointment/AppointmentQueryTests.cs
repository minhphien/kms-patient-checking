﻿using MediatR;
using Moq;
using PatientChecking.Feature.Appointment.Queries;
using PatientChecking.ServiceModels;
using PatientChecking.Services.Appointment;
using PatientChecking.Views.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PatientCheckIn.Tests.Feature.Appointment
{
    public class AppointmentQueryTests
    {
        private List<DataAccess.Models.Appointment> AppointmentDataTest()
        {
            var appointments = new List<DataAccess.Models.Appointment>
            {
                new DataAccess.Models.Appointment
                {
                    AppointmentId = 1, MedicalConcerns = "Head", CheckInDate = new DateTime(2021, 08, 26), Status = "CheckIn", PatientId = 1,
                },
                new DataAccess.Models.Appointment
                {
                    AppointmentId = 2, MedicalConcerns = "Hand", CheckInDate = new DateTime(2021, 08, 27), Status = "Cancel", PatientId = 2,
                },
                new DataAccess.Models.Appointment
                {
                    AppointmentId = 3, MedicalConcerns = "Stomach", CheckInDate = new DateTime(2021, 08, 25), Status = "Closed", PatientId = 3,
                },
                new DataAccess.Models.Appointment
                {
                    AppointmentId = 4, MedicalConcerns = "Lung", CheckInDate = new DateTime(2021, 08, 25), Status = "CheckIn", PatientId = 4,
                }
            };
            return appointments;
        }
        private AppointmentList PagingData(List<DataAccess.Models.Patient> patients)
        {
            var data = new AppointmentList
            {
                Appointments = new List<PatientChecking.ServiceModels.Appointment>
                {
                    new PatientChecking.ServiceModels.Appointment
                    {
                    AppointmentId = 1,
                    MedicalConcerns = "Head",
                    CheckInDate = new DateTime(2021, 08, 26),
                    Status = "CheckIn",
                    PatientId = patients[0].PatientId,
                    Patient =  new PatientChecking.ServiceModels.Patient{ AvatarLink = patients[0].AvatarLink, DoB = patients[0].DoB, FullName = patients[0].FullName, PatientIdentifier = patients[0].PatientIdentifier }
                    },

                    new PatientChecking.ServiceModels.Appointment
                    {
                    AppointmentId = 2,
                    MedicalConcerns = "Hand",
                    CheckInDate = new DateTime(2021, 08, 27),
                    Status = "Cancel",
                    PatientId = patients[1].PatientId,
                    Patient =  new PatientChecking.ServiceModels.Patient{ AvatarLink = patients[1].AvatarLink, DoB = patients[1].DoB, FullName = patients[1].FullName, PatientIdentifier = patients[1].PatientIdentifier }
                    },
                },
                TotalCount = 2,
            };
            return data;
        }
        private List<DataAccess.Models.Patient> PatientDataTest()
        {
            var patients = new List<DataAccess.Models.Patient>
            {
                new DataAccess.Models.Patient {PatientId = 1, PatientIdentifier = "KMS.0001", FirstName = "Long", MiddleName = "Thanh", LastName = "Do", FullName = "Long Thanh Do",
                            DoB = new DateTime(1999,11,09), Gender = 0, PhoneNumber = "0905512324", Email = "longtdo@kms-technology.com", MaritalStatus = false,
                            Nationality = "Vietnamese", EthnicRace = "Kinh", HomeTown = "Da Nang", BirthplaceCity = "Ho Chi Minh", IdcardNo = "201754622",
                            IssuedDate = new DateTime(2014, 09, 14), IssuedPlace = "Da Nang", InsuranceNo = "201329231", AvatarLink = "/Image/avatar.jpg"},

                new DataAccess.Models.Patient {PatientId = 2, PatientIdentifier = "KMS.0002", FirstName = "Duc", MiddleName = "Van", LastName = "Tran", FullName = "Duc Van Tran",
                            DoB = new DateTime(1999,05,10), Gender = 0, PhoneNumber = "0905879425", Email = "ducvant@kms-technology.com", MaritalStatus = false,
                            Nationality = "Vietnamese", EthnicRace = "Kinh", HomeTown = "Da Nang", BirthplaceCity = "Da Nang", IdcardNo = "201251266",
                            IssuedDate = new DateTime(2013, 08, 16), IssuedPlace = "Da Nang", InsuranceNo = "203125325", AvatarLink = null},
            };

            return patients;
        }

        private PagingRequest DataRequest()
        {
            var request = new PagingRequest
            {
                PageIndex = 1,
                PageSize = 2,
                SortSelection = 1,
            };
            return request;
        }

        [Fact]
        public async Task GetAppointmentByIdQuery_ExsitAppointment_ReturnsAppointmentDetailViewModel()
        {
            //Arange
            var appointments = AppointmentDataTest();
            var expected = appointments[0];
            var appointmentServices = new Mock<IAppointmentService>();
            appointmentServices.Setup(x => x.GetAppointmentByIdAsync(1)).ReturnsAsync(expected);
            var appointment = new GetAppointmentByIdQuery { Id = 1 };
            var handler = new GetAppointmentByHandler(appointmentServices.Object);
            var cts = new CancellationToken();    

            //Act
            var actual = await handler.Handle(appointment, cts);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.AppointmentId, actual.AppointmentId);
            Assert.Equal(expected.CheckInDate.ToString("yyyy-MM-dd"), actual.CheckInDate);
            Assert.Equal(expected.MedicalConcerns, actual.MedicalConcerns);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.PatientId, actual.PatientId);
        }

        [Fact]
        public async Task GetAppointmentByIdQuery_AppointmentNotFound_ReturnsNull()
        {
            //Arange
            var appointments = AppointmentDataTest();
            var appointmentServices = new Mock<IAppointmentService>();
            appointmentServices.Setup(x => x.GetAppointmentByIdAsync(1000)).ReturnsAsync((DataAccess.Models.Appointment)null); ;
            var appointment = new GetAppointmentByIdQuery { Id = 1000 };
            var handler = new GetAppointmentByHandler(appointmentServices.Object);
            var cts = new CancellationToken();

            //Act
            var actual = await handler.Handle(appointment, cts);

            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetAppointmentPagingQuery_PagingRequest_ReturnsAppointmentListViewModel()
        {
            //Arange
            var patients = PatientDataTest();
            var request = DataRequest();
            var expected = PagingData(patients);

            var mockAppointmentServices = new Mock<IAppointmentService>();
            mockAppointmentServices.Setup(x => x.GetListAppoinmentsPagingAsync(request)).ReturnsAsync(expected);
            var query = new GetAppointmentPagingQuery
            {
                pagingRequest = request
            };
            var handler = new GetAppointmentPagingHandler(mockAppointmentServices.Object);
            var cts = new CancellationToken();

            //Act
            var actual = await handler.Handle(query, cts);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.TotalCount, actual.TotalCount);
            Assert.Equal(expected.Appointments.Count, actual.AppointmentViewModels.Count);
            Assert.True(expected.Appointments.All(x => actual.AppointmentViewModels.Any(y => x.AppointmentId == y.AppointmentId && x.CheckInDate.ToString("dd-MM-yyyy")== y.CheckInDate 
                                                                                          && x.Patient.DoB.ToString("dd-MM-yyyy") == y.DoB && x.Patient.FullName == y.FullName 
                                                                                          && x.Patient.PatientIdentifier == y.PatientIdentifier 
                                                                                          && x.Status == y.Status && x.Patient.AvatarLink == y.AvatarLink)));
        }
    }
}
