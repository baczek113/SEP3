package com.example.databaseserver.DTOs;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class UpdateCompanyDto {
    private long id;
    private String name;
    private String description;
    private String website;
    private String city;
    private String postcode;
    private String address;
    private long companyRepresentativeId;
}
