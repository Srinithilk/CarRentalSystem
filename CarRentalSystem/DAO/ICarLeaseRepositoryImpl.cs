﻿using CarRentalSystem.Entity;
using CarRentalSystem.myexceptions;
using CarRentalSystem.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CarRentalSystem.DAO
{

    public class ICarLeaseRepositoryImpl : ICarLeaseRepository
    {
        SqlConnection conn;
        public void AddCar(Vehicle car)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = "INSERT INTO Vehicle (make, model, year, dailyRate, status, passengerCapacity, engineCapacity) VALUES (@make, @model, @year, @dailyRate, @status, @passengerCapacity, @engineCapacity); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@make", car.Make);
                cmd.Parameters.AddWithValue("@model", car.Model);
                cmd.Parameters.AddWithValue("@year", car.Year);
                cmd.Parameters.AddWithValue("@dailyRate", car.DailyRate);
                cmd.Parameters.AddWithValue("@status", car.Status);
                cmd.Parameters.AddWithValue("@passengerCapacity", car.PassengerCapacity);
                cmd.Parameters.AddWithValue("@engineCapacity", car.EngineCapacity);
                car.VehicleID = Convert.ToInt32(cmd.ExecuteScalar());

                if (car.VehicleID > 0)
                {
                    Console.WriteLine("----------------------------------Car added successfully------------------------------------");
                }
                else
                {
                    Console.WriteLine("----------------------------------Failed to add the car---------------------------------------");
                }
            }
            catch (Exception ex)
            {
                throw; // Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close(); // Ensure to close connection in the end
            }
        }

        public List<Vehicle> ListAllCars()
        {

            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();

                string query = "SELECT * FROM Vehicle"; // Retrieve all cars
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                List<Vehicle> allCars = new List<Vehicle>();
                while (dr.Read())
                {
                    Vehicle car = new Vehicle
                    {
                        VehicleID = dr.GetInt32(0),
                        Make = dr.GetString(1),
                        Model = dr.GetString(2),
                        Year = dr.GetInt32(3),
                        DailyRate = (double)dr.GetDecimal(4),
                        Status = dr.GetString(5),
                        PassengerCapacity = dr.GetInt32(6),
                        EngineCapacity = dr.GetInt32(7)

                    };

                    allCars.Add(car);
                }

                return allCars;
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public void AddCustomer(Customer customer)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();

                string query = "INSERT INTO Customer (firstName, lastName, email, phoneNumber) VALUES (@FirstName, @LastName, @Email, @PhoneNumber)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("---------------------------Customer added successfully----------------------------------");
                }
                else
                {
                    Console.WriteLine("--------------------------Failed to add customer-------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public Lease CreateLease(int customerID, int carID, DateTime startDate, DateTime endDate)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = "INSERT INTO Lease (customerID, vehicleID, startDate, endDate) VALUES (@CustomerID, @CarID, @StartDate, @EndDate); SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                cmd.Parameters.AddWithValue("@CarID", carID);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                Lease generatedLeaseID = new Lease();
                generatedLeaseID.LeaseID = Convert.ToInt32(cmd.ExecuteScalar());

                if (generatedLeaseID.LeaseID > 0)
                {
                    Console.WriteLine("---------------------------------Lease created successfully--------------------------------------");
                    return new Lease
                    {
                        LeaseID = generatedLeaseID.LeaseID,
                        CustomerID = customerID,
                        VehicleID = carID,
                        StartDate = startDate,
                        EndDate = endDate
                    };
                }
                else
                {
                    Console.WriteLine("--------------------------------Failed to create lease--------------------------------------------");
                    return null; // Return null or throw an exception based on your business logic
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public Vehicle FindCarById(int carID)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = $@"SELECT * FROM Vehicle WHERE vehicleID = {carID};";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Vehicle
                    {
                        VehicleID = dr.GetInt32(0),
                        Make = dr.GetString(1),
                        Model = dr.GetString(2),
                        Year = dr.GetInt32(3),
                        DailyRate = (double)dr.GetDecimal(4),
                        Status = dr.GetString(5),
                        PassengerCapacity = dr.GetInt32(6),
                        EngineCapacity = dr.GetInt32(7)
                    };
                }
                else
                {
                    throw new CarNotFoundException();
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Exception: {ex.Message}");
                throw;

            }
            finally
            {
                conn.Close(); // Ensure to close connection in the end
            }
        }

        public Customer FindCustomerById(int customerID)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = $@"SELECT * FROM Customer WHERE CustomerID = {customerID}";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Customer
                    {
                        CustomerID = dr.GetInt32(0),
                        FirstName = dr.GetString(1),
                        LastName = dr.GetString(2),
                        Email = dr.GetString(3),
                        PhoneNumber = dr.GetString(4)
                    };
                }
                else
                {
                    throw new CustomerNotFoundException();
                }
            }
            catch (Exception ex)
            {
                throw; // Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close(); // Ensure to close connection in the end
            }
        }
        public bool UpdateCustomerInformation(Customer customer)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();

                // SQL query to delete the customer by ID
                string query = $@"UPDATE Customer 
                          SET FirstName = '{customer.FirstName}', 
                              LastName = '{customer.LastName}', 
                              Email = '{customer.Email}', 
                              PhoneNumber = '{customer.PhoneNumber}' 
                          WHERE CustomerID = {customer.CustomerID}";
                SqlCommand cmd = new SqlCommand(query, conn);

                // Execute the query
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw; // Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close(); // Close connection
            }
        }

        public Lease FindLeaseById(int leaseid)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = $@"SELECT * FROM Lease WHERE LeaseID = {leaseid}";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Lease
                    {
                        LeaseID = dr.GetInt32(0),
                        VehicleID = dr.GetInt32(1),
                        CustomerID = dr.GetInt32(2),
                        StartDate = dr.GetDateTime(3),
                        EndDate = dr.GetDateTime(4),
                        Type = dr.GetString(5)
                    };
                }
                else
                {
                    throw new LeaseNotFoundException();
                }
            }
            catch (Exception ex)
            {
                throw; // Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close(); // Ensure to close connection in the end
            }
        }
        public List<Lease> ListActiveLeases()
        {
            List<Lease> activeLeases = new List<Lease>();
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = "SELECT * FROM Lease WHERE StartDate <= GETDATE() AND EndDate >= GETDATE()"; // Assuming leases within start and end dates are considered active
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Lease lease = new Lease
                    {
                        LeaseID = dr.GetInt32(0),
                        VehicleID = dr.GetInt32(1),
                        CustomerID = dr.GetInt32(2),
                        StartDate = dr.GetDateTime(3),
                        EndDate = dr.GetDateTime(4),
                        Type = dr.GetString(5)
                    };
                    activeLeases.Add(lease);
                }
                return activeLeases;
            }
            catch (Exception ex)
            {
                throw;// Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close(); // Ensure to close connection in the end
            }
        }

        public void LeaseCalculator(int leaseID)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = $@"SELECT LeaseId, Type, StartDate, EndDate FROM Lease  WHERE LeaseID = {leaseID}";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int leaseId = reader.GetInt32(0);
                    string type = reader.GetString(1);
                    DateTime startDate = reader.GetDateTime(2);
                    DateTime endDate = reader.GetDateTime(3);
                    double totalCost = CalculateLeaseCost(type, startDate, endDate);
                    Console.WriteLine($"Total Cost of Lease ID: {leaseId}, Type: {type}, Total Cost: {totalCost}");
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }


        //Concrete Method
        public double CalculateLeaseCost(string type, DateTime startDate, DateTime endDate)
        {
            double dailyRate = 50.00;  // Adjust as needed
            double monthlyRate = 1200.00;  // Adjust as needed

            if (type == "Daily")
            {
                TimeSpan duration = endDate - startDate;
                int days = (int)duration.TotalDays;
                return dailyRate * days;
            }
            else if (type == "Monthly")
            {
                int months = endDate.Month - startDate.Month + 1;
                return monthlyRate * months;
            }
            else
            {
                throw new ArgumentException("Invalid lease type");
            }
        }

        public List<Payment> RetrievePaymentHistory(int custid)
        {
            List<Payment> p = new List<Payment>();
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = $@"SELECT p.PaymentID,p.PaymentDate, p.Amount, l.LeaseId, c.FirstName, c.LastName
                            FROM Payment p
                            INNER JOIN Lease l ON p.LeaseId = l.LeaseId
                            INNER JOIN Customer c ON l.CustomerId = c.CustomerID
                            WHERE l.CustomerId = {custid}";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Payment payment = new Payment
                        {
                            PaymentID = reader.GetInt32(0),
                            LeaseID = reader.GetInt32(3),
                            PaymentDate = reader.GetDateTime(1),
                            Amount = Convert.ToDouble(reader.GetDecimal(2))
                        };
                        p.Add(payment);
                    }
                    return p;
                }
                else
                {
                    throw new CustomerNotFoundException();

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

        }
        public decimal CalculateTotalRevenue()
        {
            decimal totalRevenue = 0;
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = @"SELECT SUM(Amount) FROM Payment";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    totalRevenue = (decimal)result;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
            return totalRevenue;

        }

        public List<Vehicle> ListAvailableCars()
        {
            List<Vehicle> listavailableCars = new List<Vehicle>();
            try
            {

                conn = UtilClass.GetConnection();
                conn.Open();
                string query = "SELECT * FROM Vehicle WHERE Status = 'available'"; // Assuming leases within start and end dates are considered active
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Vehicle lease = new Vehicle
                    {
                        VehicleID = dr.GetInt32(0),
                        Make = dr.GetString(1),
                        Model = dr.GetString(2),
                        Year = dr.GetInt32(3),
                        DailyRate = (double)dr.GetDecimal(4),
                        Status = dr.GetString(5),
                        PassengerCapacity = dr.GetInt32(6),
                        EngineCapacity = dr.GetInt32(7)
                    };

                    listavailableCars.Add(lease);

                }

                return listavailableCars;
            }

            catch (Exception ex)
            {
                throw;// Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close(); // Ensure to close connection in the end
            }
        }

        public List<Customer> ListCustomers()
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = "SELECT * FROM Customer";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                List<Customer> customer = new List<Customer>();
                while (dr.Read())
                {

                    Customer cust = new Customer
                    {
                        CustomerID = dr.GetInt32(0),
                        FirstName = dr.GetString(1),
                        LastName = dr.GetString(2),
                        Email = dr.GetString(3),
                        PhoneNumber = dr.GetString(4)
                    };
                    customer.Add(cust);

                }
                return customer;

            }
            catch (Exception ex)
            {
                throw; // Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close();
            }
        }

        public List<Lease> ListLeaseHistory()
        {
            List<Lease> listLeaseHistory = new List<Lease>();
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = "SELECT * FROM Lease ";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Lease lease = new Lease
                    {
                        LeaseID = dr.GetInt32(0),
                        VehicleID = dr.GetInt32(1),
                        CustomerID = dr.GetInt32(2),
                        StartDate = dr.GetDateTime(3),
                        EndDate = dr.GetDateTime(4),
                        Type = dr.GetString(5)
                    };

                    listLeaseHistory.Add(lease);

                }

                return listLeaseHistory;
            }

            catch (Exception ex)
            {
                throw;// Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close();
            }
        }

        public List<Vehicle> ListRentedCars()
        {
            List<Vehicle> rentedCars = new List<Vehicle>();
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = "SELECT * FROM Vehicle WHERE status = 'notAvailable'"; // Fetch all cars with status as 'notAvailable' (i.e., rented out)
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Vehicle car = new Vehicle
                    {
                        VehicleID = dr.GetInt32(0),
                        Make = dr.GetString(1),
                        Model = dr.GetString(2),
                        Year = dr.GetInt32(3),
                        DailyRate = (double)dr.GetDecimal(4),
                        Status = dr.GetString(5),
                        PassengerCapacity = dr.GetInt32(6),
                        EngineCapacity = dr.GetInt32(7)
                    };

                    rentedCars.Add(car);
                }

                return rentedCars;
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                throw; // Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close(); // Ensure to close connection in the end
            }
        }

        public void RecordPayment(Lease lease, double amount)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();

                string query = "INSERT INTO Payment (LeaseID, Amount) VALUES (@LeaseID, @Amount);SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, conn);

                // Add parameters
                cmd.Parameters.AddWithValue("@LeaseID", lease.LeaseID);
                cmd.Parameters.AddWithValue("@Amount", amount);

                // Execute the query
                int generatedid = Convert.ToInt32(cmd.ExecuteScalar());
                if (generatedid > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------------------------------------------------------------");
                    Console.WriteLine("                                     Payment Recorded Successfully                                 ");
                    Console.WriteLine("-------------------------------------------------------------------------------------------------\n");
                }
                else
                {
                    Console.WriteLine("----------------------------------------Failed to create Payment-----------------------------------");
                }
            }
            catch (Exception ex)
            {
                throw; // Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close(); // Close connection
            }
        }

        public void RemoveCar(int carID)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = "DELETE FROM Vehicle WHERE VehicleID = @CarID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CarID", carID);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("--------------------------------------------------------------------------------------------------");
                    Console.WriteLine("                                       Car Removed Successfully                                  ");
                    Console.WriteLine("-------------------------------------------------------------------------------------------------\n");
                }
                else
                {
                    throw new CarNotFoundException();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public void RemoveCustomer(int customerID)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();
                string query = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustomerID", customerID);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("--------------------------------------------------------------------------------------------------");
                    Console.WriteLine("                                Customer removed successfully                                    ");
                    Console.WriteLine("-------------------------------------------------------------------------------------------------\n");
                }
                else
                {
                    throw new CustomerNotFoundException();
                }
            }
            catch (Exception ex)
            {

                throw; // Rethrow to allow handling in calling code
            }
            finally
            {
                conn.Close(); // Close connection
            }
        }

        public void ReturnCar(int leaseID)
        {
            try
            {
                conn = UtilClass.GetConnection();
                conn.Open();

                // Check if the car is already returned
                string checkQuery = $@"
            SELECT Vehicle.Status
            FROM Vehicle
            INNER JOIN Lease ON Vehicle.VehicleID = Lease.VehicleID
            WHERE Lease.LeaseID = {leaseID}";

                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                object statusResult = checkCmd.ExecuteScalar();

                if (statusResult != null && statusResult.ToString().ToLower() == "returned")
                {
                    Console.WriteLine("Car has already been returned.");
                    return;
                }

                // Update the car status
                string updateQuery = $@"
            UPDATE Vehicle
            SET Status = 'returned'
            FROM Vehicle
            INNER JOIN Lease ON Vehicle.VehicleID = Lease.VehicleID
            WHERE Lease.LeaseID = {leaseID}";

                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Car returned successfully.");
                }
                else
                {
                    throw new LeaseNotFoundException();
                }
            }
            catch (LeaseNotFoundException l)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
                // Handle exceptions, log errors, etc.
            }
            finally
            {
                conn.Close();
            }
        }

    }



}
