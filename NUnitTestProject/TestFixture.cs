﻿using System;
using NUnit.Framework;
using NuoDb.Data.Client;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Threading;
using System.Transactions;
using System.Globalization;
using System.Collections.Generic;

namespace NUnitTestProject
{
    [TestFixture]
    public class TestFixture1
    {
        static string host = "localhost:48004";
        static string user = "dba";
        static string password = "goalie";
        static string database = "test";
        static string schema = "hockey";
        static internal string connectionString = "Server=  " + host + "; Database=\"" + database + "\"; User = " + user + " ;Password   = " + password + ";Schema=\"" + schema + "\""+";SQLEngine=omega";

        [TestFixtureSetUp]
        public static void Init()
        {
            Utils.CreateHockeyTable();
        }

        [Test]
        public void TestHighAvailability()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString.Replace("Server=", "Server=localhost:8,")))
            {
                DbCommand command = new NuoDbCommand("select * from hockey", connection);

                connection.Open();
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", reader[0], reader[1], reader[2], reader["id"]);
                }
                reader.Close();
            }
        }

        [Test]
        public void TestClientInfo()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString + ";ClientInfo=hello"))
            {
                DbCommand cmd = new NuoDbCommand("select * from system.connections where connid=GetConnectionID()", connection);
                connection.Open();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    Assert.IsTrue(reader.Read());
                    try
                    {
                        Assert.AreEqual("hello", reader["clientinfo"]);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // the version of NuoDB doesn't expose the client info columns
                    }
                    Assert.IsFalse(reader.Read());
                }
            }
        }

        [Test]
        public void TestReadOnly()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString + ";ApplicationIntent=readOnly"))
            {
                DbCommand cmd = new NuoDbCommand("insert into hockey (number, team) values (9999, 'none')", connection);
                connection.Open();
                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    Assert.Fail("Read-only connection inserted {0} rows", rows);
                }
                catch (Exception e)
                {
                    Assert.IsTrue(
                        String.Compare("attempted update on readonly connection", e.Message) == 0 ||
                        String.Compare("Read only transactions cannot change data", e.Message) == 0
                        );
                }
            }
        }

        [Test]
        public void TestCommand1()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = new NuoDbCommand("select * from hockey", connection);

                connection.Open();
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", reader[0], reader[1], reader[2], reader["id"]);
                }
                reader.Close();
            }
        }

        [Test]
        public void TestCommand2()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = "select * from hockey";

                connection.Open();
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i > 0)
                            Console.Out.Write(", ");
                        Console.Out.Write(reader[i]);
                    }
                    Console.WriteLine();
                }
                reader.Close();
            }
        }

        [Test]
        public void TestParameter()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "select * from hockey where number = ?";
                command.Prepare();
                command.Parameters[0].Value = "2";

                DbDataReader reader = command.ExecuteReader();
                Assert.IsFalse(reader.Read());
                reader.Close();
            }
        }

        [Test]
        public void TestNamedParameters1()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "select * from hockey where number = ?.number and team = ?.team";
                command.Prepare();
                command.Parameters[0].Value = 1;
                command.Parameters[1].Value = "Bruins";

                DbDataReader reader = command.ExecuteReader();
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(1, reader["number"]);
                Assert.IsFalse(reader.Read());
                reader.Close();
            }
        }

        [Test]
        public void TestNamedParameters2()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "select * from hockey where number = ?.number and team = ?.team";
                NuoDbParameter p1 = new NuoDbParameter();
                p1.ParameterName = "team";
                p1.Value = "Bruins";
                command.Parameters.Add(p1);
                NuoDbParameter p2 = new NuoDbParameter();
                p2.ParameterName = "number";
                p2.Value = 1;
                command.Parameters.Add(p2);
                command.Prepare();

                DbDataReader reader = command.ExecuteReader();
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(1, reader["number"]);
                Assert.IsFalse(reader.Read());
                reader.Close();
            }
        }

        [Test]
        public void TestNamedParameters3()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "select name as \"'?\" from \"hockey\" where name='? ?.fake' or number = ?.number and team = ?.team";
                NuoDbParameter p1 = new NuoDbParameter();
                p1.ParameterName = "TEAM";
                p1.Value = "Bruins";
                command.Parameters.Add(p1);
                NuoDbParameter p2 = new NuoDbParameter();
                p2.ParameterName = "NUMBER";
                p2.Value = 1;
                command.Parameters.Add(p2);
                command.Prepare();

                DbDataReader reader = command.ExecuteReader();
                Assert.IsTrue(reader.Read());
                Assert.AreEqual("MAX SUMMIT", reader["'?"]);
                Assert.IsFalse(reader.Read());
                reader.Close();
            }
        }

        [Test]
        public void TestNamedParameters4()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "select * from hockey where number = ?.number and team = ?.team";
                command.Prepare();
                command.Parameters["NumbER"].Value = 1;
                command.Parameters["TEam"].Value = "Bruins";

                DbDataReader reader = command.ExecuteReader();
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(1, reader["NUMBER"]);
                Assert.IsFalse(reader.Read());
                reader.Close();
            }
        }

        [Test]
        public void TestNamedParameters5()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "select * from hockey where number = @number and team = @team";
                command.Prepare();
                command.Parameters["NumbER"].Value = 1;
                command.Parameters["TEam"].Value = "Bruins";

                DbDataReader reader = command.ExecuteReader();
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(1, reader["NUMBER"]);
                Assert.IsFalse(reader.Read());
                reader.Close();
            }
        }

        [Test]
        public void TestNamedParameters6()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "select * from hockey where number = @number and team = @team";
                command.Parameters.Add(new NuoDbParameter() { ParameterName = "@TEam", Value = "Bruins" });
                command.Parameters.Add(new NuoDbParameter() { ParameterName = "@NumbER", Value = 1 });

                DbDataReader reader = command.ExecuteReader();
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(1, reader["Number"]);
                Assert.IsFalse(reader.Read());
                reader.Close();
            }
        }

        [Test]
        public void TestPrepareNoParameter()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "select * from hockey where number = 2";
                command.Prepare();

                DbDataReader reader = command.ExecuteReader();
                Assert.IsFalse(reader.Read());
                reader.Close();
            }
        }

        [Test]
        public void TestPrepareDDLNoParameter()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "create table xyz (col int)";
                command.Prepare();

                try
                {
                    int value = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Assert.Fail("Executing a prepared DDL that doesn't use parameters reports an error: {0}", e.Message);
                }
                finally
                {
                    Utils.DropTable(connection, "xyz");
                }
            }
        }

        [Test]
        public void TestTransactionScope()
        {
            int count1 = -1;
            using (TransactionScope scope = new TransactionScope())
            {
                using (NuoDbConnection connection = new NuoDbConnection(connectionString))
                {
                    connection.Open();

                    DbCommand countCommand = connection.CreateCommand();
                    countCommand.CommandText = "select count(*) from hockey";

                    DbCommand updateCommand = connection.CreateCommand();
                    updateCommand.CommandText = "insert into hockey (number, name) values (99, 'xxxx')";

                    count1 = Utils.ToInt(countCommand.ExecuteScalar());
                    updateCommand.ExecuteNonQuery();
                    int count2 = Utils.ToInt(countCommand.ExecuteScalar());

                    Assert.AreEqual(count2, count1 + 1);

                    // don't call scope.Complete(), so that the transaction is aborted
                }
            }
            // verify that the data hasn't been changed
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();

                DbCommand countCommand = connection.CreateCommand();
                countCommand.CommandText = "select count(*) from hockey";

                int count3 = Utils.ToInt(countCommand.ExecuteScalar());
                Assert.AreEqual(count3, count1);
            }
        }

        [Test]
        public void TestTransactions()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand countCommand = connection.CreateCommand();
                countCommand.CommandText = "select count(*) from hockey";

                DbCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "insert into hockey (number, name) values (99, 'xxxx')";

                int count1 = Utils.ToInt(countCommand.ExecuteScalar());
                updateCommand.ExecuteNonQuery();
                int count2 = Utils.ToInt(countCommand.ExecuteScalar());

                Assert.AreEqual(count2, count1 + 1);

                transaction.Rollback();

                int count3 = Utils.ToInt(countCommand.ExecuteScalar());
                Assert.AreEqual(count3, count1);
            }
        }

        [Test]
        public void TestInsertWithGeneratedKeys()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand maxIdCmd = connection.CreateCommand();
                maxIdCmd.CommandText = "select max(id) from hockey";
                long maxId = ((IConvertible)maxIdCmd.ExecuteScalar()).ToInt64(null);

                DbCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "insert into hockey (number, name) values (99, 'xxxx')";

                DbDataReader reader = updateCommand.ExecuteReader();
                Assert.IsNotNull(reader, "The command should return a generated keys recordset");
                Assert.IsTrue(reader.Read(), "There must be at least one ID in the generated keys recordset");
                long lastId = (long)reader.GetValue(0);
                Assert.IsTrue(lastId > maxId, "The generated ID must be greater than the existing ones");

                transaction.Rollback();
            }
        }

        [Test]
        public void TestUpdateWithGeneratedKeys()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "update hockey set number = 99 where team = 'Bruins'";

                DbDataReader reader = updateCommand.ExecuteReader();
                Assert.IsNotNull(reader, "The command should return a generated keys recordset");
                Assert.IsFalse(reader.Read(), "The generated keys recordset should be empty");

                transaction.Rollback();
            }
        }

        [Test]
        public void TestPreparedInsertWithGeneratedKeys1()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand maxIdCmd = connection.CreateCommand();
                maxIdCmd.CommandText = "select max(id) from hockey";
                long maxId = ((IConvertible)maxIdCmd.ExecuteScalar()).ToInt64(null);

                DbCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "insert into hockey (number, name) values (?, ?)";
                updateCommand.Parameters.Add(99);
                updateCommand.Parameters.Add("xxx");

                DbDataReader reader = updateCommand.ExecuteReader();
                Assert.IsNotNull(reader, "The command should return a generated keys recordset");
                Assert.IsTrue(reader.Read(), "There must be at least one ID in the generated keys recordset");
                long lastId = (long)reader.GetValue(0);
                Assert.IsTrue(lastId > maxId, "The generated ID must be greater than the existing ones");

                DbCommand selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "select name from hockey where id = ?";
                selectCommand.Parameters.Add(lastId);
                string value = (string)selectCommand.ExecuteScalar();
                Assert.AreEqual("xxx", value);

                transaction.Rollback();
            }
        }

        [Test]
        public void TestPreparedInsertWithGeneratedKeys2()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand maxIdCmd = connection.CreateCommand();
                maxIdCmd.CommandText = "select max(id) from hockey";
                long maxId = ((IConvertible)maxIdCmd.ExecuteScalar()).ToInt64(null);

                DbCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "insert into hockey (number, name) values (?, ?)";
                updateCommand.Prepare();
                updateCommand.Parameters[0].Value = 99;
                updateCommand.Parameters[1].Value = "xxx";

                DbDataReader reader = updateCommand.ExecuteReader();
                Assert.IsNotNull(reader, "The command should return a generated keys recordset");
                Assert.IsTrue(reader.Read(), "There must be at least one ID in the generated keys recordset");
                long lastId = (long)reader.GetValue(0);
                Assert.IsTrue(lastId > maxId, "The generated ID must be greater than the existing ones");

                DbCommand selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "select name from hockey where id = ?";
                selectCommand.Parameters.Add(lastId);
                string value = (string)selectCommand.ExecuteScalar();
                Assert.AreEqual("xxx", value);

                transaction.Rollback();
            }
        }

        [Test]
        public void TestPreparedUpdateWithGeneratedKeys1()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "update hockey set number = 99 where team = ?";
                updateCommand.Parameters.Add("Bruins");

                DbDataReader reader = updateCommand.ExecuteReader();
                Assert.IsNotNull(reader, "The command should return a generated keys recordset");
                Assert.IsFalse(reader.Read(), "The generated keys recordset should be empty");

                transaction.Rollback();
            }
        }

        [Test]
        public void TestPreparedUpdateWithGeneratedKeys2()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();

                DbCommand updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "update hockey set number = 99 where team = ?";
                updateCommand.Prepare();
                updateCommand.Parameters[0].Value = "Bruins";

                DbDataReader reader = updateCommand.ExecuteReader();
                Assert.IsNotNull(reader, "The command should return a generated keys recordset");
                Assert.IsFalse(reader.Read(), "The generated keys recordset should be empty");

                transaction.Rollback();
            }
        }

        [Test]
        public void TestDbProvider()
        {
            DbProviderFactory factory = new NuoDbProviderFactory();
            using (DbConnection cn = factory.CreateConnection())
            {
                DbConnectionStringBuilder conStrBuilder = factory.CreateConnectionStringBuilder();
                conStrBuilder["Server"] = host;
                conStrBuilder["User"] = user;
                conStrBuilder["Password"] = password;
                conStrBuilder["Schema"] = schema;
                conStrBuilder["Database"] = database;
                Console.WriteLine("Connection string = {0}", conStrBuilder.ConnectionString);

                cn.ConnectionString = conStrBuilder.ConnectionString;
                cn.Open();

                DbCommand cmd = factory.CreateCommand();
                cmd.Connection = cn;
                cmd.CommandText = "select * from hockey";

                DbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", reader[0], reader[1], reader[2], reader["id"]);
                }
                reader.Close();
            }
        }

        [Test]
        public void TestDisconnected()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DataAdapter da = new NuoDbDataAdapter("select * from hockey", connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    for (int i = 0; i < r.ItemArray.Length; i++)
                    {
                        if (i > 0)
                            Console.Out.Write(", ");
                        Console.Out.Write(r.ItemArray[i]);
                    } // for
                    Console.Out.WriteLine();
                } // foreach 

                DataTable hockey = ds.Tables[0];
                var query = from player in hockey.AsEnumerable()
                            where player.Field<string>("Position") == "Fan"
                            select new
                            {
                                Name = player.Field<string>("Name")
                            };
                int count = 0;
                foreach (var item in query)
                {
                    Console.Out.Write(item.Name);
                    count++;
                }
                Assert.AreEqual(1, count);

            }
        }

        [Test]
        public void TestDisconnectedUpdate()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbDataAdapter da = new NuoDbDataAdapter("select id, number, name, position, team from hockey", connection);
                NuoDbCommandBuilder builder = new NuoDbCommandBuilder();
                builder.DataAdapter = da;
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataRow row = dt.NewRow();
                row["NAME"] = "John Doe";
                row["POSITION"] = "Developer";
                row["TEAM"] = "NuoDB";
                row["NUMBER"] = 100;
                dt.Rows.Add(row);

                int changed = da.Update(dt);
                Assert.AreEqual(1, changed);

                // TODO: http://msdn.microsoft.com/en-us/library/ks9f57t0%28v=vs.80%29.aspx describes a few options
                // to retrieve the AutoNumber column. For the moment, I reload the entire table
                dt = new DataTable();
                da.Fill(dt);

                DataRow[] rows = dt.Select("NUMBER = 100");
                Assert.IsNotNull(rows);
                Assert.AreEqual(1, rows.Length);
                foreach (DataRow r in rows)
                    r["NUMBER"] = 0;
                changed = da.Update(dt);
                Assert.AreEqual(1, changed);

                rows = dt.Select("NUMBER = 0");
                Assert.IsNotNull(rows);
                Assert.AreEqual(1, rows.Length);
                foreach (DataRow r in rows)
                    r.Delete();
                changed = da.Update(dt);
                Assert.AreEqual(1, changed);

            }
        }

        public void TestDataType(string sqlType, object value)
        {
            TestDataType(sqlType, value, value);
        }

        public void TestDataType(string sqlType, object value, object expected)
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                //DbTransaction transaction = connection.BeginTransaction();

                Utils.DropTable(connection, "temp");

                DbCommand createCommand = new NuoDbCommand("create table temp (col " + sqlType + ")", connection);
                int result = createCommand.ExecuteNonQuery();

                DbCommand insertCommand = new NuoDbCommand("insert into temp (col) values (?)", connection);
                insertCommand.Parameters.Add(value);
                int inserted = insertCommand.ExecuteNonQuery();

                DbCommand command = new NuoDbCommand("select col from temp", connection);
                object val = command.ExecuteScalar();
                // compare dates using the string representation
                if (val.GetType() == expected.GetType())
                    Assert.AreEqual(expected, val);
                else if (val is DateTime)
                    Assert.AreEqual(DateTime.Parse(expected.ToString()), val);
                else if (val is TimeSpan)
                    Assert.AreEqual(TimeSpan.Parse(expected.ToString()), val);
                else if (expected is ICollection)
                    CollectionAssert.AreEqual((ICollection)expected, (ICollection)val);
                else
                    Assert.AreEqual(expected, val);

                //transaction.Rollback();
            }
        }

        [Test]
        public void TestDataTypeString()
        {
            TestDataType("string", "dummy");
            TestDataType("varchar(255)", "dummy");
            //TestDataType("longvarchar", "dummy");
            TestDataType("clob", "dummy");
        }

        [Test]
        public void TestDataTypeBoolean()
        {
            TestDataType("boolean", false);
        }

        [Test]
        public void TestDataTypeByte()
        {
            //TestDataType("tinyint", 45);
        }

        [Test]
        public void TestDataTypeInt16()
        {
            TestDataType("smallint", 45);
        }

        [Test]
        public void TestDataTypeInt32()
        {
            TestDataType("integer", 45);
            TestDataType("int", 45);
            TestDataType("int", -9);
            TestDataType("integer", -45);
            TestDataType("integer", 0);
            TestDataType("integer", 4500);
        }

        [Test]
        public void TestDataTypeInt64()
        {
            TestDataType("bigint", 45000000000);
        }

        [Test]
        public void TestDataTypeFloat()
        {
            TestDataType("real", 45.3f);
            TestDataType("float", 45.3f);
        }

        [Test]
        public void TestDataTypeDouble()
        {
            TestDataType("double", 45.3987654321);
        }

        [Test]
        public void TestDataTypeDecimal()
        {
            TestDataType("numeric", 45.3987654321, 45);
            TestDataType("numeric(18,12)", 45.3987654321M);
            TestDataType("numeric(18,3)", 45.3987654321, 45.399M);
            TestDataType("decimal(18,12)", 45.3987654321M);
            TestDataType("numeric(18,12)", 0.0000000045M);
            TestDataType("numeric(18,12)", -0.0000000045M);
            TestDataType("dec(18,12)", 45.3987654321M);
            TestDataType("numeric(22,5)", Decimal.Parse("12345678901234567.89999", new CultureInfo("en-US")));
            TestDataType("numeric(22,5)", Decimal.Parse("-12345678901234567.89999", new CultureInfo("en-US")));
            TestDataType("numeric(26)", Decimal.Parse("12345678901234567900000000", new CultureInfo("en-US")));
            TestDataType("numeric(26)", Decimal.Parse("-12345678901234567900000000", new CultureInfo("en-US")));
        }

        [Test]
        public void TestDataTypeChar()
        {
            TestDataType("char", 'A', "A");
        }

        [Test]
        public void TestDataTypeDate()
        {
            DateTime now = DateTime.Now;
            TestDataType("date", now, now.Date);
            TestDataType("date", "1999-01-31");
        }

        [Test]
        public void TestDataTypeTime()
        {
            DateTime now = DateTime.Now;
            TestDataType("time", now, now.TimeOfDay);
            TestDataType("time", new TimeSpan(10, 30, 22));
            TestDataType("time", "10:30:22");
        }

        [Test]
        public void TestDataTypeTimestamp()
        {
            DateTime now = DateTime.Now;
            TestDataType("timestamp", now);
            TestDataType("timestamp", "1999-01-31 10:30:00.100");
            TestDataType("datetime", "1999-01-31 10:30:00.100");
        }

        [Test]
        public void TestDataTypeBlob()
        {
            TestDataType("blob", "xxx", new byte[] { (byte)'x', (byte)'x', (byte)'x' });
            TestDataType("blob", new byte[] { (byte)'x', (byte)'x', (byte)'x' }, new byte[] { (byte)'x', (byte)'x', (byte)'x' });
            TestDataType("blob", "\x00\x01\x02", new byte[] { 0, 1, 2 });
            TestDataType("blob", new byte[] { 3, 0, 2 }, new byte[] { 3, 0, 2 });
        }

        [Test]
        public void TestSchema()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DataTable tables = connection.GetSchema("Tables");

                Boolean found = false;
                foreach (DataRow row in tables.Rows)
                {
                    if (row.Field<string>(2).Equals("HOCKEY"))
                        found = true;
                }
                Assert.IsTrue(found, "Table HOCKEY was not found in the list of tables");
            }
        }

        [Test]
        public void TestScalability()
        {
            using (NuoDbConnection cnn = new NuoDbConnection(connectionString))
            {
                cnn.Open();
                Utils.DropTable(cnn, "temp");

                DbCommand createCommand = new NuoDbCommand("create table temp (col1 integer, col2 integer)", cnn);
                int result = createCommand.ExecuteNonQuery();

                DbCommand cmm = cnn.CreateCommand();
                cmm.CommandText = "insert into temp(col1, col2) values(?, ?)";
                cmm.Parameters.Add(new NuoDbParameter { DbType = DbType.Int32, ParameterName = "col1" });
                cmm.Parameters.Add(new NuoDbParameter { DbType = DbType.Int32, ParameterName = "col2" });
                cmm.Prepare();

                int[] count = new int[] { 1000, 5000, 10000, 20000, 40000 };
                double[] times = new double[count.Length];
                for (var k = 0; k < count.Length; k++)
                {
                    DateTime start = DateTime.Now;
                    for (var i = 1; i <= count[k]; i++)
                    {
                        cmm.Parameters["col1"].Value = i;
                        cmm.Parameters["col2"].Value = 2 * i;
                        cmm.ExecuteNonQuery();
                    }
                    DateTime end = DateTime.Now;
                    times[k] = (end - start).TotalMilliseconds;
                    if (k == 0)
                        Console.WriteLine("{0} runs = {1} msec", count[k], times[k]);
                    else
                    {
                        double countRatio = (count[k] / count[0]);
                        double timeRatio = (times[k] / times[0]);
                        Console.WriteLine("{0} runs = {1} msec => {2} {3}", count[k], times[k], countRatio, timeRatio);
                        Assert.IsTrue(timeRatio < (countRatio * 1.50), "Scalability at {2} rows is not linear! (time for {0} rows = {1}; time for {2} rows = {3} => ratio = {4} is greater than {5}",
                            new object[] { count[0], times[0], count[k], times[k], timeRatio, countRatio });

                    }
                }
            }
        }

        private static void CreateTargetForBulkLoad()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                Utils.DropTable(connection, "temp");

                DbCommand createCommand = new NuoDbCommand("create table temp (col string)", connection);
                int result = createCommand.ExecuteNonQuery();
            }
        }

        private static void VerifyBulkLoad(int expectedCount, string expectedFirstRow)
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();

                DbCommand command = new NuoDbCommand("select count(*) from temp", connection);
                object val = command.ExecuteScalar();

                Assert.AreEqual(expectedCount, Utils.ToInt(val));

                command = new NuoDbCommand("select col from temp", connection);
                val = command.ExecuteScalar();

                Assert.AreEqual(expectedFirstRow, val);
            }
        }

        [Test]
        public void TestBulkLoad_DataRowsNoMapping()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = schema + ".TEMP";
            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz", typeof(string));
            DataRow[] rows = new DataRow[10];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = metadata.NewRow();
                rows[i][0] = Convert.ToString(i);
            }

            loader.WriteToServer(rows);

            VerifyBulkLoad(rows.Length, "0");
        }

        [Test]
        public void TestBulkLoad_DataRowsWithMappingOrdinal2Ordinal()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add(1, 0);

            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz1", typeof(int));
            metadata.Columns.Add("xyz2", typeof(string));
            DataRow[] rows = new DataRow[10];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = metadata.NewRow();
                rows[i][0] = -1;
                rows[i][1] = Convert.ToString(i);
            }

            loader.WriteToServer(rows);

            VerifyBulkLoad(rows.Length, "0");
        }

        [Test]
        public void TestBulkLoad_DataRowsWithMappingOrdinal2Name()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add(1, "col");

            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz1", typeof(int));
            metadata.Columns.Add("xyz2", typeof(string));
            DataRow[] rows = new DataRow[10];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = metadata.NewRow();
                rows[i][0] = -1;
                rows[i][1] = Convert.ToString(i);
            }

            loader.WriteToServer(rows);

            VerifyBulkLoad(rows.Length, "0");
        }

        [Test]
        public void TestBulkLoad_DataRowsWithMappingName2Ordinal()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add("xyz2", 0);

            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz1", typeof(int));
            metadata.Columns.Add("xyz2", typeof(string));
            DataRow[] rows = new DataRow[10];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = metadata.NewRow();
                rows[i][0] = -1;
                rows[i][1] = Convert.ToString(i);
            }

            loader.WriteToServer(rows);

            VerifyBulkLoad(rows.Length, "0");
        }

        [Test]
        public void TestBulkLoad_DataRowsWithMappingName2Name()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add("xyz2", "col");

            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz1", typeof(int));
            metadata.Columns.Add("xyz2", typeof(string));
            DataRow[] rows = new DataRow[10];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = metadata.NewRow();
                rows[i][0] = -1;
                rows[i][1] = Convert.ToString(i);
            }

            loader.WriteToServer(rows);

            VerifyBulkLoad(rows.Length, "0");
        }

        [Test]
        public void TestBulkLoad_DataTableWithStateNoMapping()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz", typeof(string));
            const int ROW_TO_ADD = 10;
            metadata.BeginLoadData();
            for (int i = 0; i < ROW_TO_ADD; i++)
            {
                DataRow row = metadata.NewRow();
                row[0] = Convert.ToString(i);
                metadata.Rows.Add(row);
            }
            metadata.EndLoadData();
            metadata.AcceptChanges();
            metadata.Rows[ROW_TO_ADD / 2].BeginEdit();
            metadata.Rows[ROW_TO_ADD / 2][0] = "999";
            metadata.Rows[ROW_TO_ADD / 2].EndEdit();

            loader.WriteToServer(metadata, DataRowState.Modified);

            VerifyBulkLoad(1, "999");
        }

        [Test]
        public void TestBulkLoad_DataTableNoMapping()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz", typeof(string));
            const int ROW_TO_ADD = 10;
            for (int i = 0; i < ROW_TO_ADD; i++)
            {
                DataRow row = metadata.NewRow();
                row[0] = Convert.ToString(i);
                metadata.Rows.Add(row);
            }

            loader.WriteToServer(metadata);

            VerifyBulkLoad(ROW_TO_ADD, "0");
        }

        [Test]
        public void TestBulkLoad_DataTableWithMappingOrdinal2Ordinal()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add(1, 0);

            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz1", typeof(int));
            metadata.Columns.Add("xyz2", typeof(string));
            const int ROW_TO_ADD = 10;
            for (int i = 0; i < ROW_TO_ADD; i++)
            {
                DataRow row = metadata.NewRow();
                row[0] = -1;
                row[1] = Convert.ToString(i);
                metadata.Rows.Add(row);
            }

            loader.WriteToServer(metadata);

            VerifyBulkLoad(ROW_TO_ADD, "0");
        }

        [Test]
        public void TestBulkLoad_DataTableWithMappingOrdinal2Name()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add(1, "col");

            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz1", typeof(int));
            metadata.Columns.Add("xyz2", typeof(string));
            const int ROW_TO_ADD = 10;
            for (int i = 0; i < ROW_TO_ADD; i++)
            {
                DataRow row = metadata.NewRow();
                row[0] = -1;
                row[1] = Convert.ToString(i);
                metadata.Rows.Add(row);
            }

            loader.WriteToServer(metadata);

            VerifyBulkLoad(ROW_TO_ADD, "0");
        }

        [Test]
        public void TestBulkLoad_DataTableWithMappingName2Ordinal()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add("xyz2", 0);

            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz1", typeof(int));
            metadata.Columns.Add("xyz2", typeof(string));
            const int ROW_TO_ADD = 10;
            for (int i = 0; i < ROW_TO_ADD; i++)
            {
                DataRow row = metadata.NewRow();
                row[0] = -1;
                row[1] = Convert.ToString(i);
                metadata.Rows.Add(row);
            }

            loader.WriteToServer(metadata);

            VerifyBulkLoad(ROW_TO_ADD, "0");
        }

        [Test]
        public void TestBulkLoad_DataTableWithMappingName2Name()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add("xyz2", "col");

            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz1", typeof(int));
            metadata.Columns.Add("xyz2", typeof(string));
            const int ROW_TO_ADD = 10;
            for (int i = 0; i < ROW_TO_ADD; i++)
            {
                DataRow row = metadata.NewRow();
                row[0] = -1;
                row[1] = Convert.ToString(i);
                metadata.Rows.Add(row);
            }

            loader.WriteToServer(metadata);

            VerifyBulkLoad(ROW_TO_ADD, "0");
        }

        [Test]
        public void TestBulkLoad_DataReaderNoMapping()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";

            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = new NuoDbCommand("select position from hockey order by number", connection);

                connection.Open();
                DbDataReader reader = command.ExecuteReader();
                loader.WriteToServer(reader);
                reader.Close();

                command = new NuoDbCommand("select count(*) from hockey", connection);
                object val = command.ExecuteScalar();
                VerifyBulkLoad(Utils.ToInt(val), "Fan");
            }
        }

        [Test]
        public void TestBulkLoad_DataReaderWithMappingOrdinal2Ordinal()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add(1, 0);

            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = new NuoDbCommand("select number, position as xyz2 from hockey order by number", connection);

                connection.Open();
                DbDataReader reader = command.ExecuteReader();
                loader.WriteToServer(reader);
                reader.Close();

                command = new NuoDbCommand("select count(*) from hockey", connection);
                object val = command.ExecuteScalar();
                VerifyBulkLoad(Utils.ToInt(val), "Fan");
            }

        }

        [Test]
        public void TestBulkLoad_DataReaderWithMappingOrdinal2Name()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add(1, "col");

            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = new NuoDbCommand("select number, position as xyz2 from hockey order by number", connection);

                connection.Open();
                DbDataReader reader = command.ExecuteReader();
                loader.WriteToServer(reader);
                reader.Close();

                command = new NuoDbCommand("select count(*) from hockey", connection);
                object val = command.ExecuteScalar();
                VerifyBulkLoad(Utils.ToInt(val), "Fan");
            }

        }

        [Test]
        public void TestBulkLoad_DataReaderWithMappingName2Ordinal()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add("xyz2", 0);

            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = new NuoDbCommand("select number, position as xyz2 from hockey order by number", connection);

                connection.Open();
                DbDataReader reader = command.ExecuteReader();
                loader.WriteToServer(reader);
                reader.Close();

                command = new NuoDbCommand("select count(*) from hockey", connection);
                object val = command.ExecuteScalar();
                VerifyBulkLoad(Utils.ToInt(val), "Fan");
            }
        }

        [Test]
        public void TestBulkLoad_DataReaderWithMappingName2Name()
        {
            CreateTargetForBulkLoad();

            NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
            loader.BatchSize = 2;
            loader.DestinationTableName = "TEMP";
            loader.ColumnMappings.Add("xyz2", "col");

            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                DbCommand command = new NuoDbCommand("select number, position as xyz2 from hockey order by number", connection);

                connection.Open();
                DbDataReader reader = command.ExecuteReader();
                loader.WriteToServer(reader);
                reader.Close();

                command = new NuoDbCommand("select count(*) from hockey", connection);
                object val = command.ExecuteScalar();
                VerifyBulkLoad(Utils.ToInt(val), "Fan");
            }
        }

        [Test]
        public void TestBulkLoadPerformance()
        {
            using (NuoDbConnection cnn = new NuoDbConnection(connectionString))
            {
                cnn.Open();
                Utils.DropTable(cnn, "temp");

                DbCommand createCommand = new NuoDbCommand("create table temp (col1 integer, col2 integer)", cnn);
                int result = createCommand.ExecuteNonQuery();

                DbCommand cmm = cnn.CreateCommand();
                cmm.CommandText = "insert into temp(col1, col2) values(?, ?)";
                cmm.Parameters.Add(new NuoDbParameter { DbType = DbType.Int32, ParameterName = "col1" });
                cmm.Parameters.Add(new NuoDbParameter { DbType = DbType.Int32, ParameterName = "col2" });
                cmm.Prepare();

                const int ROW_NUMBER = 40000;
                DateTime start = DateTime.Now;
                for (var i = 1; i <= ROW_NUMBER; i++)
                {
                    cmm.Parameters["col1"].Value = i;
                    cmm.Parameters["col2"].Value = 2 * i;
                    cmm.ExecuteNonQuery();
                }
                DateTime end = DateTime.Now;
                double insertTime = (end - start).TotalMilliseconds;

                Utils.DropTable(cnn, "temp2");
                createCommand = new NuoDbCommand("create table temp2 (col1 integer, col2 integer)", cnn);
                createCommand.ExecuteNonQuery();

                NuoDbBulkLoader loader = new NuoDbBulkLoader(connectionString);
                loader.DestinationTableName = "TEMP2";

                DbCommand command = new NuoDbCommand("select * from temp", cnn);
                DbDataReader reader = command.ExecuteReader();

                loader.BatchProcessed += new BatchProcessedEventHandler(loader_BatchProcessed);
                start = DateTime.Now;
                loader.WriteToServer(reader);
                end = DateTime.Now;

                double loadTime = (end - start).TotalMilliseconds;

                reader.Close();

                Console.WriteLine("{0} insert = {1}\n{0} bulk load = {2}\n", ROW_NUMBER, insertTime, loadTime);

                Assert.IsTrue(loadTime < insertTime, "BulkLoad takes more time than manual insertion");
            }
        }

        void loader_BatchProcessed(object sender, NuoDb.Data.Client.BatchProcessedEventArgs e)
        {
            Console.WriteLine("Batch of {0} rows inserted, current count is {1}\n", e.BatchSize, e.TotalSize);
        }

        [Test]
        public void TestConnectionPooling()
        {
            NuoDbConnectionStringBuilder builder = new NuoDbConnectionStringBuilder(connectionString);
            builder.Pooling = true;
            builder.ConnectionLifetime = 2;
            String newConnString = builder.ConnectionString;
            int pooledItems = 0;
            NuoDbConnection.ClearAllPools();
            using (NuoDbConnection cnn = new NuoDbConnection(newConnString))
            {
                cnn.Open();

                // 1 busy
                pooledItems = NuoDbConnection.GetPooledConnectionCount(cnn);
                Assert.AreEqual(1, pooledItems);

                using (NuoDbConnection cnn2 = new NuoDbConnection(newConnString))
                {
                    cnn2.Open();

                    // 2 busy
                    pooledItems = NuoDbConnection.GetPooledConnectionCount(cnn);
                    Assert.AreEqual(2, pooledItems);
                }

                // 1 available, 1 busy
                pooledItems = NuoDbConnection.GetPooledConnectionCount(cnn);
                Assert.AreEqual(2, pooledItems);

                Thread.Sleep(13000);

                // 1 busy
                pooledItems = NuoDbConnection.GetPooledConnectionCount(cnn);
                Assert.AreEqual(1, pooledItems);
            }

            // 1 available
            pooledItems = NuoDbConnection.GetPooledConnectionCount(newConnString);
            Assert.AreEqual(1, pooledItems);

            using (NuoDbConnection cnn = new NuoDbConnection(newConnString))
            {
                cnn.Open();

                // 1 busy
                pooledItems = NuoDbConnection.GetPooledConnectionCount(cnn);
                Assert.AreEqual(1, pooledItems);
            }

            // 1 available
            pooledItems = NuoDbConnection.GetPooledConnectionCount(newConnString);
            Assert.AreEqual(1, pooledItems);

            Thread.Sleep(13000);

            // empty pool
            pooledItems = NuoDbConnection.GetPooledConnectionCount(newConnString);
            Assert.AreEqual(0, pooledItems);
        }

        [Test]
        public void TestConnectionPoolingMaxAge()
        {
            NuoDbConnectionStringBuilder builder = new NuoDbConnectionStringBuilder(connectionString);
            builder.Pooling = true;
            builder.ConnectionLifetime = 2;
            builder.MaxLifetime = 3;
            String newConnString = builder.ConnectionString;
            int pooledItems = 0;
            NuoDbConnection.ClearAllPools();
            using (NuoDbConnection cnn = new NuoDbConnection(newConnString))
            {
                cnn.Open();

                // 1 busy
                pooledItems = NuoDbConnection.GetPooledConnectionCount(cnn);
                Assert.AreEqual(1, pooledItems);

                Thread.Sleep(2000);
            }

            // 1 available
            pooledItems = NuoDbConnection.GetPooledConnectionCount(newConnString);
            Assert.AreEqual(1, pooledItems);

            using (NuoDbConnection cnn = new NuoDbConnection(newConnString))
            {
                cnn.Open();

                // 1 busy
                pooledItems = NuoDbConnection.GetPooledConnectionCount(cnn);
                Assert.AreEqual(1, pooledItems);

                Thread.Sleep(2000);
            }

            // 0 available, the connection is too old to be recycled
            pooledItems = NuoDbConnection.GetPooledConnectionCount(newConnString);
            Assert.AreEqual(0, pooledItems);
        }

        private void SimulateLoad(string connectionString)
        {
            using (NuoDbConnection cnn = new NuoDbConnection(connectionString))
            {
                cnn.Open();

                Thread.Sleep(2000);
            }
        }

        [Test]
        public void TestConnectionPoolingMaxConnections()
        {
            NuoDbConnectionStringBuilder builder = new NuoDbConnectionStringBuilder(connectionString);
            builder.Pooling = true;
            builder.MaxConnections = 2;
            String newConnString = builder.ConnectionString;
            NuoDbConnection.ClearAllPools();
            DateTime start = DateTime.Now;
            // start two long (simulated) queries
            Thread t1 = new Thread(() => SimulateLoad(newConnString));
            t1.Start();
            Thread t2 = new Thread(() => SimulateLoad(newConnString));
            t2.Start();
            Thread.Sleep(500);
            int pooledItems = NuoDbConnection.GetPooledConnectionCount(newConnString);
            Assert.AreEqual(2, pooledItems);
            // try to open a third connection: it should stall until one of the threads ends
            using (NuoDbConnection cnn = new NuoDbConnection(newConnString))
            {
                cnn.Open();
                DateTime end = DateTime.Now;
                Assert.GreaterOrEqual(end - start, TimeSpan.FromMilliseconds(2000));

                pooledItems = NuoDbConnection.GetPooledConnectionCount(newConnString);
                Assert.LessOrEqual(pooledItems, 2);
            }

        }

        [Test]
        public void TestAsynchronousReader1()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                NuoDbCommand command = new NuoDbCommand("select * from hockey", connection);

                connection.Open();
                IAsyncResult result = command.BeginExecuteReader();

                using (DbDataReader reader = command.EndExecuteReader(result))
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", reader[0], reader[1], reader[2], reader["id"]);
                    }
                }
            }
        }

        [Test]
        public void TestAsynchronousReader2()
        {
            NuoDbConnection connection = new NuoDbConnection(connectionString);
            NuoDbCommand command = new NuoDbCommand("select * from hockey", connection);

            connection.Open();

            AsyncCallback callback = new AsyncCallback(HandleCallback);
            IAsyncResult result = command.BeginExecuteReader(callback, command);
        }

        void HandleCallback(IAsyncResult result)
        {
            NuoDbCommand command = (NuoDbCommand)result.AsyncState;
            using (DbDataReader reader = command.EndExecuteReader(result))
            {
                while (reader.Read())
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", reader[0], reader[1], reader[2], reader["id"]);
                }
            }
            command.Close();
            command.Connection.Close();
        }

        [Test]
        public void TestAsynchronousScalar1()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                NuoDbCommand countCommand = (NuoDbCommand)connection.CreateCommand();
                countCommand.CommandText = "select count(*) from hockey";

                connection.Open();

                IAsyncResult result = countCommand.BeginExecuteScalar();

                int count = Utils.ToInt(countCommand.EndExecuteScalar(result));
            }
        }

        [Test]
        public void TestAsynchronousScalar2()
        {
            NuoDbConnection connection = new NuoDbConnection(connectionString);
            NuoDbCommand countCommand = (NuoDbCommand)connection.CreateCommand();
            countCommand.CommandText = "select count(*) from hockey";

            connection.Open();

            AsyncCallback callback = new AsyncCallback(HandleCallback2);
            IAsyncResult result = countCommand.BeginExecuteScalar(callback, countCommand);
        }

        [Test]
        public void TestAsynchronousScalar3()
        {
            NuoDbConnection connection = new NuoDbConnection(connectionString);
            NuoDbCommand countCommand = (NuoDbCommand)connection.CreateCommand();
            countCommand.CommandText = "select count(*) from hockey";

            connection.Open();
            AsyncCallback callback = new AsyncCallback(HandleCallback2);
            for (int i = 0; i < 20; i++)
                countCommand.BeginExecuteScalar(callback, countCommand);
        }

        void HandleCallback2(IAsyncResult result)
        {
            NuoDbCommand command = (NuoDbCommand)result.AsyncState;
            int count = Utils.ToInt(command.EndExecuteScalar(result));
        }

        [Test]
        public void TestAsynchronousUpdate1()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                Utils.DropTable(connection, "temp");

                NuoDbCommand createCommand = new NuoDbCommand("create table temp (col string)", connection);
                IAsyncResult result = createCommand.BeginExecuteNonQuery();

                int count = createCommand.EndExecuteNonQuery(result);
            }
        }


        [Test]
        public void TestAsynchronousUpdate2()
        {
            NuoDbConnection connection = new NuoDbConnection(connectionString);
            connection.Open();
            Utils.DropTable(connection, "temp");

            NuoDbCommand countCommand = (NuoDbCommand)connection.CreateCommand();
            countCommand.CommandText = "create table temp (col string)";

            AsyncCallback callback = new AsyncCallback(HandleCallback3);
            IAsyncResult result = countCommand.BeginExecuteNonQuery(callback, countCommand);
        }

        void HandleCallback3(IAsyncResult result)
        {
            NuoDbCommand command = (NuoDbCommand)result.AsyncState;
            try
            {
                int count = command.EndExecuteNonQuery(result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Utils.DropTable(command.Connection as NuoDbConnection, "temp");
            command.Close();
            command.Connection.Close();
        }

#if !MONO
        [Test]
        public void TestTimeZone()
        {
            // Use a time in the UTC time zone; otherwise, it would be treated as if it were in the local timezone even
            // if we are telling NuoDB that we are in a different timezone
            DateTime dstReferenceDate = DateTime.SpecifyKind(new DateTime(1999, 10, 1, 2, 30, 58), DateTimeKind.Utc);
            DateTime nonDstReferenceDate = DateTime.SpecifyKind(new DateTime(1999, 12, 1, 2, 30, 58), DateTimeKind.Utc);
            DateTime dtDate;
            string strDate;
            bool hasNext;
            // GMT-5, or GMT-4 if DST is active
            using (NuoDbConnection connection = new NuoDbConnection(connectionString + ";TimeZone=America/New_York"))
            {
                connection.Open();
                Utils.DropTable(connection, "timezone");

                DbCommand createCommand = new NuoDbCommand("create table timezone (asTimestamp timestamp, asDate date, asTime time, asString string)", connection);
                int result = createCommand.ExecuteNonQuery();

                DbCommand insertCommand = new NuoDbCommand("insert into timezone (asTimestamp, asDate, asTime, asString) values (?,?,?,?)", connection);
                insertCommand.Parameters.Add(dstReferenceDate);
                insertCommand.Parameters.Add(dstReferenceDate);
                insertCommand.Parameters.Add(dstReferenceDate);
                insertCommand.Parameters.Add(dstReferenceDate);
                insertCommand.ExecuteNonQuery();
                insertCommand.Parameters.Clear();
                insertCommand.Parameters.Add(nonDstReferenceDate);
                insertCommand.Parameters.Add(nonDstReferenceDate);
                insertCommand.Parameters.Add(nonDstReferenceDate);
                insertCommand.Parameters.Add(nonDstReferenceDate);
                insertCommand.ExecuteNonQuery();

                DbCommand command = new NuoDbCommand("select asTimestamp, asDate, asTime, asString from timezone", connection);
                DbDataReader reader = command.ExecuteReader();
                hasNext = reader.Read();
                Assert.IsTrue(hasNext);
                dtDate = reader.GetDateTime(0);
                Assert.AreEqual("1999-09-30 22:30:58", dtDate.ToString("yyyy-MM-dd HH:mm:ss"));
                strDate = reader.GetString(0);
                Assert.AreEqual("1999-09-30 22:30:58", strDate);
                dtDate = reader.GetDateTime(1);
                Assert.AreEqual("1999-09-30", dtDate.ToString("yyyy-MM-dd"));
                strDate = reader.GetString(1);
                Assert.AreEqual("1999-09-30", strDate);
                dtDate = reader.GetDateTime(2);
                Assert.AreEqual("22:30:58", dtDate.ToString("HH:mm:ss"));
                strDate = reader.GetString(2);
                Assert.AreEqual("22:30:58", strDate);
                dtDate = reader.GetDateTime(3);
                Assert.AreEqual("1999-09-30 22:30:58", dtDate.ToString("yyyy-MM-dd HH:mm:ss"));
                strDate = reader.GetString(3);
                Assert.AreEqual("1999-09-30 22:30:58", strDate);

                hasNext = reader.Read();
                Assert.IsTrue(hasNext);
                dtDate = reader.GetDateTime(0);
                Assert.AreEqual("1999-11-30 21:30:58", dtDate.ToString("yyyy-MM-dd HH:mm:ss"));
                strDate = reader.GetString(0);
                Assert.AreEqual("1999-11-30 21:30:58", strDate);
                dtDate = reader.GetDateTime(1);
                Assert.AreEqual("1999-11-30", dtDate.ToString("yyyy-MM-dd"));
                strDate = reader.GetString(1);
                Assert.AreEqual("1999-11-30", strDate);
                dtDate = reader.GetDateTime(2);
                Assert.AreEqual("21:30:58", dtDate.ToString("HH:mm:ss"));
                strDate = reader.GetString(2);
                Assert.AreEqual("21:30:58", strDate);
                dtDate = reader.GetDateTime(3);
                Assert.AreEqual("1999-11-30 21:30:58", dtDate.ToString("yyyy-MM-dd HH:mm:ss"));
                strDate = reader.GetString(3);
                Assert.AreEqual("1999-11-30 21:30:58", strDate);
            }
            // all the date-based columns should magically move one hour back when we change timezone
            // GMT-6, or GMT-5 if DST is active
            using (NuoDbConnection connection = new NuoDbConnection(connectionString + ";TimeZone=America/Chicago"))
            {
                connection.Open();
                DbCommand command = new NuoDbCommand("select asTimestamp, asDate, asTime, asString from timezone", connection);
                DbDataReader reader = command.ExecuteReader();
                hasNext = reader.Read();
                Assert.IsTrue(hasNext);
                dtDate = reader.GetDateTime(0);
                Assert.AreEqual("1999-09-30 21:30:58", dtDate.ToString("yyyy-MM-dd HH:mm:ss"));
                strDate = reader.GetString(0);
                Assert.AreEqual("1999-09-30 21:30:58", strDate);
                dtDate = reader.GetDateTime(1);
                Assert.AreEqual("1999-09-30", dtDate.ToString("yyyy-MM-dd"));
                strDate = reader.GetString(1);
                Assert.AreEqual("1999-09-30", strDate);
                dtDate = reader.GetDateTime(2);
                Assert.AreEqual("21:30:58", dtDate.ToString("HH:mm:ss"));
                strDate = reader.GetString(2);
                Assert.AreEqual("21:30:58", strDate);
                dtDate = reader.GetDateTime(3);
                Assert.AreEqual("1999-09-30 22:30:58", dtDate.ToString("yyyy-MM-dd HH:mm:ss"));
                strDate = reader.GetString(3);
                Assert.AreEqual("1999-09-30 22:30:58", strDate);

                hasNext = reader.Read();
                Assert.IsTrue(hasNext);
                dtDate = reader.GetDateTime(0);
                Assert.AreEqual("1999-11-30 20:30:58", dtDate.ToString("yyyy-MM-dd HH:mm:ss"));
                strDate = reader.GetString(0);
                Assert.AreEqual("1999-11-30 20:30:58", strDate);
                dtDate = reader.GetDateTime(1);
                Assert.AreEqual("1999-11-30", dtDate.ToString("yyyy-MM-dd"));
                strDate = reader.GetString(1);
                Assert.AreEqual("1999-11-30", strDate);
                dtDate = reader.GetDateTime(2);
                Assert.AreEqual("20:30:58", dtDate.ToString("HH:mm:ss"));
                strDate = reader.GetString(2);
                Assert.AreEqual("20:30:58", strDate);
                dtDate = reader.GetDateTime(3);
                Assert.AreEqual("1999-11-30 21:30:58", dtDate.ToString("yyyy-MM-dd HH:mm:ss"));
                strDate = reader.GetString(3);
                Assert.AreEqual("1999-11-30 21:30:58", strDate);
            }
        }
#endif

        [Test]
        public void TestDbReaderGetObject()
        {
            NuoDbConnection connection = new NuoDbConnection(connectionString);
            connection.Open();
            Utils.DropTable(connection, "temp");

            new NuoDbCommand("create table temp (col1 bigint, col2 int)", connection).ExecuteNonQuery();
            new NuoDbCommand("insert into temp values (0, 0)", connection).ExecuteNonQuery();
            DbDataReader reader = new NuoDbCommand("select * from temp", connection).ExecuteReader();
            bool hasNext = reader.Read();
            Assert.IsTrue(hasNext);
            Assert.AreEqual(0L, reader.GetInt64(0));
            Assert.AreEqual(0L, reader.GetInt64(1));
            Assert.IsTrue(typeof(long) == reader[0].GetType(), "Type of col1 is " + reader[0].GetType().Name);
            Assert.IsTrue(typeof(int) == reader[1].GetType(), "Type of col2 is " + reader[1].GetType().Name);
        }

        [Test]
        public void TestAutoClose()
        {
            int index;
            for (index = 0; index < 2000; index++)
            {
                using (var connection = new NuoDbConnection(connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "select * from hockey limit 1";
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            Assert.True(reader.Read());
                        }
                    }
                }
            }
            Assert.AreEqual(2000, index);
        }

        [Test]
        public void TestUTFParams()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                Utils.DropTable(connection, "temp");

                string utf8String = "z a \u0306 \u01FD \u03B2";
                new NuoDbCommand("create table temp (col1 string)", connection).ExecuteNonQuery();
                using (NuoDbCommand cmd = new NuoDbCommand("insert into temp values (?)", connection))
                {
                    cmd.Prepare();
                    cmd.Parameters[0].Value = utf8String;
                    Assert.AreEqual(1, cmd.ExecuteNonQuery());
                }
                using (NuoDbCommand cmd = new NuoDbCommand("select * from temp", connection))
                {
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        Assert.AreEqual(utf8String, reader.GetString(0));
                    }
                }
            }
        }

        [Test]
        public void TestBulkLoadOnCommand()
        {
            CreateTargetForBulkLoad();

            DataTable metadata = new DataTable("dummy");
            metadata.Columns.Add("xyz", typeof(string));
            DataRow[] rows = new DataRow[10];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = metadata.NewRow();
                rows[i][0] = Convert.ToString(i);
            }

            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                NuoDbCommand loader = new NuoDbCommand("insert into temp values (?) ", connection);
                
                loader.ExecuteBatch(rows);
            }

            VerifyBulkLoad(rows.Length, "0");
        }

        [Test]
        public void testDB13047()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "drop table tmp if exists";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "create table tmp (strvalue string, numvalue int)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "insert into tmp values ('first', 1), (null, null)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select * from tmp";
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        Assert.IsFalse(reader.IsDBNull(0));
                        Assert.IsFalse(reader.IsDBNull(1));

                        Assert.AreEqual("first", reader.GetString(0));
                        Assert.AreEqual("first", reader.GetValue(0));
                        Assert.AreEqual(1, reader.GetInt32(1));
                        Assert.AreEqual(1, reader.GetValue(1));
                        
                        Assert.IsTrue(reader.Read());
                        Assert.IsTrue(reader.IsDBNull(0));
                        Assert.IsTrue(reader.IsDBNull(1));

                        Assert.IsNull(reader.GetString(0));
                        Assert.IsNull(reader.GetValue(0));
                        Assert.AreEqual(0, reader.GetInt32(1));
                        Assert.IsNull(reader.GetValue(1));
                        
                        Assert.IsFalse(reader.Read());
                    }
                }
            }
        }

        [Test]
        public void testDB16326()
        {
            using (NuoDbConnection connection = new NuoDbConnection(connectionString))
            {
                connection.Open();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "drop table tmp if exists";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "create table tmp (numvalue1 decimal(15,6), numvalue2 numeric(19,10))";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "insert into tmp values (?, ?)";
                    cmd.Prepare();

                    LinkedList<decimal> values = new LinkedList<decimal>();
                    values.AddLast(0m);
                    values.AddLast(30000m);
                    values.AddLast(-2.3m);
                    values.AddLast(-1000000m);
                    values.AddLast(13m);
                    values.AddLast(0.000034m);
                    values.AddLast(-0.01m);
                    foreach (decimal d in values)
                    {
                        cmd.Parameters[0].Value = d;
                        cmd.Parameters[1].Value = d;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = "select * from tmp";
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal d0 = reader.GetDecimal(0);
                            decimal d1 = reader.GetDecimal(1);
                            Assert.AreEqual(d0, d1, "Decimal(15,6) and number store different values");
                            decimal expected = values.First.Value;
                            values.RemoveFirst();
                            Assert.AreEqual(expected, d1, "Decimal value is different from inserted one");
                        }
                    }
                }
            }
        }

    
    }

}
