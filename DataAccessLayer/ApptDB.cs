﻿using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DataAccessLayer.Logger;
using Dapper;

namespace DataAccessLayer
{
    public class ApptDB : GenericRepositoryDL<MAppointment>, IApptDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<MAppointment> _logger;
        public ApptDB(ApplicationDbContext context, ILogger<MAppointment> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MAppointment Data)
        {
            var ret = await _context.MAppointment.AnyAsync(p => p.AppointmentName.ToUpper() == Data.AppointmentName.ToUpper() && p.ApptId != Data.ApptId);
             return ret;
        }

        public Task<List<DTOAppointmentResponse>> GetALLAppt()
        {
            var GetALL = (from A in _context.MAppointment
                         //join F in _context.MFormation
                         //on A.FormationId equals F.FormationId
                        
                         select new DTOAppointmentResponse
                         {
                             ApptId=A.ApptId,
                             AppointmentName=A.AppointmentName,
                             //FormationId=F.FormationId,
                             //FormationName=F.FormationName, 


                         }).OrderByDescending(x=>x.ApptId).ToList();


            return Task.FromResult(GetALL);
        }
        public async Task<List<DTOAppointmentResponse>> GetALLByAppointmentName(string AppointmentName)
        {
            try
            {
                var GetALL = (from A in _context.MAppointment
                              where A.AppointmentName.Contains(AppointmentName)
                              select new DTOAppointmentResponse
                              {
                                  ApptId = A.ApptId,
                                  AppointmentName = A.AppointmentName,
                              }).Take(5).ToList();
                return await Task.FromResult(GetALL);
            }
            catch(Exception ex)
            {
                _logger.LogError(1001, ex, "ApptDB->GetALLByAppointmentName");
                return null;
            }

        }

        public Task<List<DTOAppointmentResponse>> GetByFormationId(int FormationId)
        {
            var GetALL = (from A in _context.MAppointment
                          //join F in _context.MFormation
                          //on A.FormationId equals F.FormationId
                         // where F.FormationId == FormationId
                          select new DTOAppointmentResponse
                          {
                              ApptId = A.ApptId,
                              AppointmentName = A.AppointmentName,
                             // FormationId = F.FormationId,
                             // FormationName = F.FormationName,


                          }).ToList();

            return Task.FromResult(GetALL);
        }
        public async Task<DTOAppointmentResponse?> GetByApptId(short ApptId)
        {
            try
            {
                var GetAppt = await (from app in _context.MAppointment.Where(x => x.ApptId == ApptId)
                               select new DTOAppointmentResponse
                               {
                                   ApptId = app.ApptId,
                                   AppointmentName = app.AppointmentName,
                               }).FirstOrDefaultAsync();

                return GetAppt;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ApptDB->GetByApptId");
                return null;
            }
        }

        //public async Task<bool> GetByName(MCorps Data)
        //{
        //    var ret = _context.MCorps.Where(p=> p.ComdId == Data.ComdId).Select(p => p.CorpsName.ToUpper() == Data.CorpsName.ToUpper()).FirstOrDefault();
        //    return ret;
        //}

        //public Task<List<DTOCorpsResponse>> GetALLCorps()
        //{
        //    var Corps = (from c in _context.MCorps
        //                 join d in _context.MComd
        //                 on c.ComdId equals d.ComdId
        //                 where c.CorpsId!=1
        //                 select new DTOCorpsResponse
        //                 {

        //                     CorpsId = c.CorpsId,
        //                     CorpsName = c.CorpsName,
        //                     comdName = d.ComdName,
        //                     ComdId=d.ComdId,

        //                 }).ToList();


        //    return Task.FromResult(Corps);  
        //}

        //public async Task<List<DTOCorpsResponse>> GetByComdId(int ComdId)
        //{
        //    var Corps = (from c in _context.MCorps
        //                 join d in _context.MComd
        //                 on c.ComdId equals d.ComdId where c.ComdId == ComdId   
        //                 select new DTOCorpsResponse
        //                 {

        //                     CorpsId = c.CorpsId,
        //                     CorpsName = c.CorpsName,



        //                 }).ToList();


        //    return await Task.FromResult(Corps);
        //}



        //public UserDB(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}
        public async Task<DTOApptIdCheckInFKTableResponse?> ApptIdCheckInFKTable(short ApptId)
        {
            try
            {
                string query = "Select count(distinct mapp.ApptId)as TotalTDM from MAppointment mapp" +
                                " left join TrnDomainMapping tdm on tdm.ApptId = mapp.ApptId " +
                                " where mapp.ApptId =@ApptId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOApptIdCheckInFKTableResponse>(query, new { ApptId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ApptDB->ApptIdCheckInFKTable");
                return null;
            }
        }
    }
}