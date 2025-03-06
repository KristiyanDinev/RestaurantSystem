﻿using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels;
using ITStepFinalProject.Models.WebModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class ReservationDatabaseHandler
    {
        private static readonly string table = "Reservations";
        private static readonly string tableRestorant = "Restorant";
        private static readonly string tableTimeTable = "TimeTable";

        public async Task<bool> CreateReservation(InsertReservationModel model, string currentStatus)
        {
            ResultSqlQuery restorant = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("*", tableRestorant)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", model.RestorantId, "=", "AND")
                .BuildCondition("ReservationMaxAdults", model.Amount_Of_Adults, ">=", "AND", " ((")
                .BuildCondition("ReservationMaxAdults", 0, ">=", "OR", "", ") ")
                .BuildCondition("ReservationMaxAdults", 0, "<", "AND", "", ") ")


                .BuildCondition("ReservationMaxChildren", model.Amount_Of_Children, ">=", "AND", " ((")
                .BuildCondition("ReservationMaxChildren", 0, ">=", "OR", "", ") ")
                .BuildCondition("ReservationMaxChildren", 0, "<", "AND", "", ") ")

                .BuildCondition("ReservationMinAdults", model.Amount_Of_Adults, "<=", "AND", " ((")
                .BuildCondition("ReservationMinAdults", 0, ">=", "OR", "", ")")
                .BuildCondition("ReservationMinAdults", 0, "<", "AND", "", ")")

                .BuildCondition("ReservationMinChildren", model.Amount_Of_Children, "<=", "AND", " ((")
                .BuildCondition("ReservationMinChildren", 0, ">=", "OR", "", ") ")
                .BuildCondition("ReservationMinChildren", 0, "<", "", "", ") ")
                .ToString(),
                new RestorantModel());
            
            if (restorant.Models.Count == 0)
            {
                return false;
            }

            model.CurrentStatus = currentStatus;
            await DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Insert(table, [model]).ToString());
            return true;
        }

        public async Task<List<DisplayReservationModel>> GetReservationsByUser(UserModel model)
        {
            ResultSqlQuery reservations = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("*", table)
                .Join(tableRestorant, "INNER")
                .ConditionKeyword("ON")
                .BuildCondition(table+ ".RestorantId", '"'+tableRestorant+"\".\"Id\"")
                .ConditionKeyword("WHERE")
                .BuildCondition("ReservatorId", model.Id)
                .ToString(), new DisplayReservationModel());

            return reservations.Models.Cast<DisplayReservationModel>().ToList();
        }

        public async void DeleteReservation(int reservationId)
        {
            await DatabaseManager._ExecuteNonQuery(
                new SqlBuilder()
                .Delete(table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", reservationId).ToString()
                );
        }

        public async Task<List<TimeTableJoinRestorantModel>> GetRestorantsAddressesForReservation(UserModel user)
        {
            string city = ValueHandler.Strings(user.City);
            string country = ValueHandler.Strings(user.Country);

            SqlBuilder sqlBuilder = new SqlBuilder()
                .Select("*", tableTimeTable)
                .Join(tableRestorant, "INNER")
                .ConditionKeyword("ON")
                .BuildCondition(tableRestorant + ".Id", '"' + tableTimeTable + "\".\"RestorantId\"")

                .ConditionKeyword("WHERE")
                .BuildCondition("ServeCustomersInPlace", "'1'", "=", "AND")
                .BuildCondition("UserCity", city, "=", "AND")
                .BuildCondition("RestorantCity", city, "=", "AND")
                .BuildCondition("UserCountry", country, "=", "AND")
                .BuildCondition("RestorantCountry", country, "=", "AND")
                .BuildCondition("UserAddress", ValueHandler.Strings('%' + user.Address + '%'), "LIKE", "AND");

            string state = ValueHandler.Strings(user.State);
            if (state.Equals("null"))
            {
                sqlBuilder.BuildCondition("UserState", "NULL", "IS", "AND");
                sqlBuilder.BuildCondition("RestorantState", "NULL", "IS");

            }
            else
            {
                sqlBuilder.BuildCondition("UserState", state, "=", "AND");
                sqlBuilder.BuildCondition("RestorantState", state, "=");
            }

            ResultSqlQuery objs = await DatabaseManager._ExecuteQuery(sqlBuilder
                .ToString(), new TimeTableJoinRestorantModel());

            return objs.Models.Cast<TimeTableJoinRestorantModel>().ToList();
        }
    }
}
