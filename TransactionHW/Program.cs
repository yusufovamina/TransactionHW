using Microsoft.Data.SqlClient;
#region DBCreation
//string conStr = @"Data Source = STHQ012E-09; Database=master; TrustServerCertificate=true; Integrated Security=false; User Id=admin; Password=admin;";

//using (SqlConnection connection = new SqlConnection(conStr))
//{
//    await connection.OpenAsync();
//    SqlCommand command = new SqlCommand();
//    command.CommandText = "CREATE DATABASE Cheloveki";
//    command.Connection = connection;
//    await command.ExecuteNonQueryAsync();
//    Console.WriteLine("Database has created!");
//}
#endregion

string conStr = @"Data Source = STHQ012E-09; Database=Cheloveki; TrustServerCertificate=true; Integrated Security=false; User Id=admin; Password=admin;" ;

#region TableCreation
//using (SqlConnection connection = new SqlConnection(conStr))
//{
//    await connection.OpenAsync();
//    SqlCommand command = new SqlCommand();
//    command.CommandText = "CREATE TABLE Cheloveki(Id INT PRIMARY KEY IDENTITY, Name NVARCHAR(30) NOT NULL, Surname NVARCHAR(30) NOT NULL, Amount MONEY NOT NULL)";
//    command.Connection = connection;
//    await command.ExecuteNonQueryAsync();
//    Console.WriteLine("Table has created!");
//}
#endregion

#region TableInsert
//using (SqlConnection connection = new SqlConnection(conStr))
//{
//    await connection.OpenAsync();
//    SqlCommand command = new SqlCommand();
//    command.CommandText = "INSERT INTO Cheloveki(Name,Surname, Amount) VALUES ('Orxan','Mammedov',2500)," +
//                                                                         " ('Salman','Quliev',767)," +
//                                                                          " ('Ayxan','Rzazade',4178)," +
//                                                                          " ('Laman','Pashaeva',2955)," +
//                                                                           " ('Ramiz','Tagiev',1834)," +
//                                                                           " ('Zara','Axmedova',1095)," +
//                                                                           " ('Ilham','Garibov',961)," +
//                                                                         " ('Ayan', 'Sadixova',1023)";
//    command.Connection = connection;
//    int responseNumber = await command.ExecuteNonQueryAsync();
//    Console.WriteLine(responseNumber + " rows added!");
//}
#endregion


#region ById
//int id1, id2;
//Console.WriteLine("Enter 1 Chelovek's Id: ");
//id1 = Convert.ToInt32(Console.ReadLine());
//Console.WriteLine("Enter 2 Chelovek's Id: ");
//id2 = Convert.ToInt32(Console.ReadLine());
//Console.WriteLine("Enter amount to make a transaction: ");
//int amount;
//amount = Convert.ToInt32(Console.ReadLine());

//await MakeTransactionByID(conStr, id1, id2, amount);
#endregion

#region ByNameSurname
string name1, surname1, name2, surname2;
Console.WriteLine("Enter 1 Chelovek's name: ");
name1 = Console.ReadLine();
Console.WriteLine("Enter 1 Chelovek's surname: ");
surname1 = Console.ReadLine();
Console.WriteLine("Enter 2 Chelovek's name: ");
name2 = Console.ReadLine();
Console.WriteLine("Enter 2 Chelovek's surname: ");
surname2 = Console.ReadLine();



Console.WriteLine("Enter amount to make a transaction: ");
int amount;
amount = Convert.ToInt32(Console.ReadLine());
await MakeTransactionByNameSurname(conStr, name1, surname1, name2, surname2, amount);
#endregion

static async Task MakeTransactionByID(string connectionStr, int FirstPersonID, int SecondPersonID, int Amount)
{
    using (SqlConnection connection = new SqlConnection(connectionStr))
    {
        await connection.OpenAsync();

        SqlTransaction transaction = connection.BeginTransaction();

        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
             command.CommandText = $"SELECT Amount From Cheloveki WHERE Id={FirstPersonID}";
            object balance= await command.ExecuteScalarAsync();
            if (Convert.ToInt32(balance) < Amount)
            {
                Console.WriteLine("Not enough balance to make transaction");
            }
            else
            {
                command.CommandText = @"UPDATE Cheloveki SET Amount = Amount - @Amount WHERE Id = @FirstPersonId";

                command.Parameters.Add("@Amount", System.Data.SqlDbType.Int);
                command.Parameters.Add("@FirstPersonId", System.Data.SqlDbType.Int);
                command.Parameters["@Amount"].Value = Amount;
                command.Parameters["@FirstPersonId"].Value = FirstPersonID;
                await command.ExecuteNonQueryAsync();


                command.CommandText = @"UPDATE Cheloveki SET Amount = Amount + @Amount  WHERE Id = @SecondPersonID";

                command.Parameters.Add("@SecondPersonID", System.Data.SqlDbType.Int);

                command.Parameters["@SecondPersonID"].Value = SecondPersonID;
                await command.ExecuteNonQueryAsync();


                await transaction.CommitAsync();
                Console.WriteLine("Succesfull transaction");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await transaction.RollbackAsync();
        }


     
        command.Connection = connection;
        await command.ExecuteNonQueryAsync();
    }
}





static async Task MakeTransactionByNameSurname(string connectionStr, string FirstPersonName, string FirstPersonSurname, string SecondPersonName, string SecondPersonSurname, int Amount)
{
    using (SqlConnection connection = new SqlConnection(connectionStr))
    {
        await connection.OpenAsync();

        SqlTransaction transaction = connection.BeginTransaction();

        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            command.CommandText = $"SELECT Amount From Cheloveki WHERE Name= '{FirstPersonName}' AND Surname='{FirstPersonSurname}'";
            object balance = await command.ExecuteScalarAsync();
            if (Convert.ToInt32(balance) < Amount)
            {
                Console.WriteLine("Not enough balance to make transaction");
            }
            else
            {
                command.CommandText = @"UPDATE Cheloveki SET Amount = Amount - @Amount WHERE Name = '@FirstPersonName' AND Surname = '@FirstPersonSurname}'";

                command.Parameters.Add("@Amount", System.Data.SqlDbType.Int);
                command.Parameters.Add("@FirstPersonName", System.Data.SqlDbType.NVarChar, 30);
                command.Parameters.Add("@FirstPersonSurname", System.Data.SqlDbType.NVarChar, 30);
                command.Parameters["@Amount"].Value = Amount;
                command.Parameters["@FirstPersonName"].Value = FirstPersonName;
                command.Parameters["@FirstPersonSurname"].Value = FirstPersonSurname;

                await command.ExecuteNonQueryAsync();

                command.CommandText = @"UPDATE Cheloveki SET Amount = Amount+ @Amount WHERE Name = '@SecondPersonName' AND Surname = '@SecondPersonSurname}'"; await command.ExecuteNonQueryAsync();
                command.Parameters.Add("@SecondPersonName", System.Data.SqlDbType.NVarChar, 30);
                command.Parameters.Add("@SecondPersonSurname", System.Data.SqlDbType.NVarChar, 30);
                command.Parameters["@SecondPersonName"].Value = SecondPersonName;
                command.Parameters["@SecondPersonSurname"].Value = SecondPersonSurname;
                await transaction.CommitAsync();
                Console.WriteLine("Succesfull transaction");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await transaction.RollbackAsync();
        }

        command.Connection = connection;
        await command.ExecuteNonQueryAsync();
    }
}

