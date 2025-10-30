package com.example.databaseserver.DB;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class DBConnection {
    private static final String address = "jdbc:postgresql://localhost:5432/postgres?currentSchema=";
    private static final String user = "postgres";
    private static final String password = "dupa123";

    public static Connection getConnection() throws SQLException{
        return DriverManager.getConnection(address, user, password);
    }
}
