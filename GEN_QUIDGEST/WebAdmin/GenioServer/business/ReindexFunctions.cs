using ExecuteQueryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CSGenio.persistence;
using CSGenio.business;
using CSGenio.framework;
using Quidgest.Persistence.GenericQuery;
using Quidgest.Persistence;

namespace CSGenio.business
{
    public class ReindexFunctions
    {
        public PersistentSupport sp { get; set; }
        public User user { get; set; }
        public bool Zero { get; set; }

        public ReindexFunctions(PersistentSupport sp, User user, bool Zero = false) {
            this.sp = sp;
            this.user = user;
            this.Zero = Zero;
        }   

        public void DeleteInvalidRows(CancellationToken cToken) {
            List<int> zzstateToRemove = new List<int> { 1, 11 };
            DataMatrix dm;
            sp.openConnection();

            /* --- PROMEM --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAmem.FldCodmem)
                .From(CSGenioAmem.AreaMEM)
                .Where(CriteriaSet.And().In(CSGenioAmem.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAmem model = new CSGenioAmem(user);
                model.ValCodmem = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- PROPAIS --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioApais.FldCodpais)
                .From(CSGenioApais.AreaPAIS)
                .Where(CriteriaSet.And().In(CSGenioApais.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioApais model = new CSGenioApais(user);
                model.ValCodpais = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- UserLogin --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioApsw.FldCodpsw)
                .From(CSGenioApsw.AreaPSW)
                .Where(CriteriaSet.And().In(CSGenioApsw.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioApsw model = new CSGenioApsw(user);
                model.ValCodpsw = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- AsyncProcess --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_apr.FldCodascpr)
                .From(CSGenioAs_apr.AreaS_APR)
                .Where(CriteriaSet.And().In(CSGenioAs_apr.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_apr model = new CSGenioAs_apr(user);
                model.ValCodascpr = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- NotificationEmailSignature --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_nes.FldCodsigna)
                .From(CSGenioAs_nes.AreaS_NES)
                .Where(CriteriaSet.And().In(CSGenioAs_nes.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_nes model = new CSGenioAs_nes(user);
                model.ValCodsigna = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- NotificationMessage --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_nm.FldCodmesgs)
                .From(CSGenioAs_nm.AreaS_NM)
                .Where(CriteriaSet.And().In(CSGenioAs_nm.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_nm model = new CSGenioAs_nm(user);
                model.ValCodmesgs = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- PROCIDAD --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAcidad.FldCodcidad)
                .From(CSGenioAcidad.AreaCIDAD)
                .Where(CriteriaSet.And().In(CSGenioAcidad.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAcidad model = new CSGenioAcidad(user);
                model.ValCodcidad = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- AsyncProcessArgument --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_arg.FldCodargpr)
                .From(CSGenioAs_arg.AreaS_ARG)
                .Where(CriteriaSet.And().In(CSGenioAs_arg.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_arg model = new CSGenioAs_arg(user);
                model.ValCodargpr = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- AsyncProcessAttachments --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_pax.FldCodpranx)
                .From(CSGenioAs_pax.AreaS_PAX)
                .Where(CriteriaSet.And().In(CSGenioAs_pax.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_pax model = new CSGenioAs_pax(user);
                model.ValCodpranx = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- UserAuthorization --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAs_ua.FldCodua)
                .From(CSGenioAs_ua.AreaS_UA)
                .Where(CriteriaSet.And().In(CSGenioAs_ua.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAs_ua model = new CSGenioAs_ua(user);
                model.ValCodua = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- PROagente_imobiliario --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAagent.FldCodagent)
                .From(CSGenioAagent.AreaAGENT)
                .Where(CriteriaSet.And().In(CSGenioAagent.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAagent model = new CSGenioAagent(user);
                model.ValCodagent = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- PROPROPR --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioApropr.FldCodpropr)
                .From(CSGenioApropr.AreaPROPR)
                .Where(CriteriaSet.And().In(CSGenioApropr.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioApropr model = new CSGenioApropr(user);
                model.ValCodpropr = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- PROALBUM --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAalbum.FldCodalbum)
                .From(CSGenioAalbum.AreaALBUM)
                .Where(CriteriaSet.And().In(CSGenioAalbum.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAalbum model = new CSGenioAalbum(user);
                model.ValCodalbum = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                

            /* --- PROCONTC --- */
            dm = sp.Execute(
                new SelectQuery()
                .Select(CSGenioAcontc.FldCodcontc)
                .From(CSGenioAcontc.AreaCONTC)
                .Where(CriteriaSet.And().In(CSGenioAcontc.FldZzstate, zzstateToRemove))
                );

            for (int i = 0; i < dm.NumRows; i++)
            {
                CSGenioAcontc model = new CSGenioAcontc(user);
                model.ValCodcontc = dm.GetKey(i, 0);

                try
                {
                    model.delete(sp);
                }
                //Not every exception should be allowed to continue record deletion, only business exceptions need to be caught and allow to deletion continue.
                //If there are other types of exceptions, such as database connection problems, for example, execution should be stopped immediately
                catch(BusinessException ex)
                {
                    Log.Error((ex.UserMessage != null) ? ex.UserMessage : ex.Message);
                }
            }
                
            
            //Hard Coded Tabels
            //These can be directly removed

            /* --- PROmem --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROmem")
                .Where(CriteriaSet.And().In("PROmem", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROcfg --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROcfg")
                .Where(CriteriaSet.And().In("PROcfg", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROlstusr --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROlstusr")
                .Where(CriteriaSet.And().In("PROlstusr", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROlstcol --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROlstcol")
                .Where(CriteriaSet.And().In("PROlstcol", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROlstren --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROlstren")
                .Where(CriteriaSet.And().In("PROlstren", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROusrwid --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROusrwid")
                .Where(CriteriaSet.And().In("PROusrwid", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROusrcfg --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROusrcfg")
                .Where(CriteriaSet.And().In("PROusrcfg", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROusrset --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROusrset")
                .Where(CriteriaSet.And().In("PROusrset", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROwkfact --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROwkfact")
                .Where(CriteriaSet.And().In("PROwkfact", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROwkfcon --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROwkfcon")
                .Where(CriteriaSet.And().In("PROwkfcon", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROwkflig --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROwkflig")
                .Where(CriteriaSet.And().In("PROwkflig", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROwkflow --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROwkflow")
                .Where(CriteriaSet.And().In("PROwkflow", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROnotifi --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROnotifi")
                .Where(CriteriaSet.And().In("PROnotifi", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROprmfrm --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROprmfrm")
                .Where(CriteriaSet.And().In("PROprmfrm", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROscrcrd --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROscrcrd")
                .Where(CriteriaSet.And().In("PROscrcrd", "ZZSTATE", zzstateToRemove)));
                
            /* --- docums --- */
            sp.Execute(new DeleteQuery()
                .Delete("docums")
                .Where(CriteriaSet.And().In("docums", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROpostit --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROpostit")
                .Where(CriteriaSet.And().In("PROpostit", "ZZSTATE", zzstateToRemove)));
                
            /* --- hashcd --- */
            sp.Execute(new DeleteQuery()
                .Delete("hashcd")
                .Where(CriteriaSet.And().In("hashcd", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROalerta --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROalerta")
                .Where(CriteriaSet.And().In("PROalerta", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROaltent --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROaltent")
                .Where(CriteriaSet.And().In("PROaltent", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROtalert --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROtalert")
                .Where(CriteriaSet.And().In("PROtalert", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROdelega --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROdelega")
                .Where(CriteriaSet.And().In("PROdelega", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROTABDINAMIC --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROTABDINAMIC")
                .Where(CriteriaSet.And().In("PROTABDINAMIC", "ZZSTATE", zzstateToRemove)));
                
            /* --- UserAuthorization --- */
            sp.Execute(new DeleteQuery()
                .Delete("UserAuthorization")
                .Where(CriteriaSet.And().In("UserAuthorization", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROaltran --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROaltran")
                .Where(CriteriaSet.And().In("PROaltran", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROworkflowtask --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROworkflowtask")
                .Where(CriteriaSet.And().In("PROworkflowtask", "ZZSTATE", zzstateToRemove)));
                
            /* --- PROworkflowprocess --- */
            sp.Execute(new DeleteQuery()
                .Delete("PROworkflowprocess")
                .Where(CriteriaSet.And().In("PROworkflowprocess", "ZZSTATE", zzstateToRemove)));
                

            sp.closeConnection();
        }




 
        // USE /[MANUAL RDX_STEP]/
    }
}