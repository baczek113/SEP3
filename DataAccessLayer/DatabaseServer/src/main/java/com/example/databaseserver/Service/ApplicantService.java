package com.example.databaseserver.Service;

import com.example.databaseserver.Entities.Applicant;
import com.example.databaseserver.Entities.User;
import com.example.databaseserver.Entities.Location;
import com.example.databaseserver.Entities.ApplicantSkill;
import com.example.databaseserver.Entities.Skill;
import com.example.databaseserver.Entities.SkillLevel;
import com.example.databaseserver.Entities.UserRole;
import com.example.databaseserver.Repositories.*;
import com.example.databaseserver.generated.*;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import net.devh.boot.grpc.server.service.GrpcService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@GrpcService
public class ApplicantService extends ApplicantServiceGrpc.ApplicantServiceImplBase {

    private final ApplicationRepository applicationRepository;
    private final ApplicantRepository applicantRepository;
    private final LocationRepository locationRepository;
    private final SkillRepository skillRepository;
    private final ApplicantSkillRepository applicantSkillRepository;
    private final UserRepository userRepository;
    private static final Logger log = LoggerFactory.getLogger(ApplicantService.class);

    @Autowired
    public ApplicantService(ApplicationRepository applicationRepository, ApplicantRepository applicantRepository, LocationRepository locationRepository, ApplicantSkillRepository applicantSkillRepository, SkillRepository skillRepository, UserRepository userRepository) {
        this.applicationRepository = applicationRepository;
        this.applicantRepository = applicantRepository;
        this.locationRepository = locationRepository;
        this.applicantSkillRepository = applicantSkillRepository;
        this.userRepository = userRepository;
        this.skillRepository = skillRepository;
    }

    @Override
    @Transactional
    public void createApplicant(CreateApplicantRequest request,
                                StreamObserver<ApplicantResponse> responseObserver) {
        if (userRepository.existsByEmail(request.getEmail())) {
            log.warn("Blocked duplicate registration attempt for email: {} | APPLICANT", request.getEmail());
            responseObserver.onError(
                    Status.ALREADY_EXISTS
                            .withDescription("Email already in use: " + request.getEmail())
                            .asRuntimeException()
            );
            return;
        }
        try {
            User user = new User(
                    request.getEmail(),
                    request.getPasswordHash(),
                    UserRole.applicant,
                    request.getName()
            );

            Location location = new Location(
                    request.getCity(),
                    request.getPostcode(),
                    request.getAddress()
            );
            Location savedLocation = locationRepository.save(location);

            Applicant applicant = new Applicant(
                    user,
                    request.getExperience(),
                    savedLocation
            );

            Applicant savedApplicant = applicantRepository.save(applicant);

            ApplicantResponse response = ApplicantResponse.newBuilder()
                    .setId(savedApplicant.getId())
                    .setName(savedApplicant.getUser().getName())
                    .setEmail(savedApplicant.getUser().getEmail())
                    .setExperience(
                            savedApplicant.getExperience() == null
                                    ? ""
                                    : savedApplicant.getExperience()
                    )
                    .setCity(savedApplicant.getLocation().getCity())
                    .setPostcode(
                            savedApplicant.getLocation().getPostcode() == null
                                    ? ""
                                    : savedApplicant.getLocation().getPostcode()
                    )
                    .setAddress(
                            savedApplicant.getLocation().getAddress() == null
                                    ? ""
                                    : savedApplicant.getLocation().getAddress()
                    )
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            responseObserver.onError(Status.INTERNAL
                    .withDescription("Error creating applicant: " + e.getMessage())
                    .asRuntimeException());
        }
    }


    @Override
    @Transactional
    public void removeApplicant(RemoveApplicantRequest request,
                                StreamObserver<RemoveApplicantResponse> responseObserver)
    {
        try {
            long id = request.getId();

            // Check if applicant exists
            var applicantOpt = applicantRepository.findById(id);
            if (applicantOpt.isEmpty()) {
                RemoveApplicantResponse response = RemoveApplicantResponse.newBuilder()
                        .setSuccess(false)
                        .setMessage("Applicant with id " + id + " not found.")
                        .build();

                responseObserver.onNext(response);
                responseObserver.onCompleted();
                return;
            }

            // 1) Delete applications for this applicant (if cascade is not already configured)
            applicationRepository.deleteByApplicantId(id);

            // 2) Delete applicant skills
            applicantSkillRepository.deleteByApplicantId(id);

            // 3) Delete applicant itself
            applicantRepository.deleteById(id);

            // 4) Optionally delete user / auth row if Applicant is linked to User
            // userRepository.deleteById(id);  // only if this matches your schema

            RemoveApplicantResponse response = RemoveApplicantResponse.newBuilder()
                    .setSuccess(true)
                    .setMessage("Applicant account deleted successfully.")
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            e.printStackTrace();
            responseObserver.onError(
                    Status.UNKNOWN
                            .withDescription(e.getMessage())
                            .withCause(e)
                            .asRuntimeException()
            );
        }
    }



    @Override
    @Transactional
    public void addApplicantSkill(AddApplicantSkillRequest request,
                                  StreamObserver<ApplicantSkillResponse> responseObserver) {
        try {
            Applicant applicant = applicantRepository.findById(request.getApplicantId())
                    .orElseThrow(() -> new RuntimeException(
                            "Applicant not found with id: " + request.getApplicantId()
                    ));

            Skill skill = skillRepository.findByName(request.getSkillName())
                    .orElseGet(() -> {
                        String category = request.getCategory().isBlank()
                                ? null
                                : request.getCategory();
                        Skill newSkill = new Skill(request.getSkillName(), category);
                        return skillRepository.save(newSkill);
                    });

            boolean exists = applicantSkillRepository
                    .existsByApplicant_IdAndSkill_Id(applicant.getId(), skill.getId());

            if (exists) {
                throw new RuntimeException("Applicant already has this skill assigned.");
            }

            SkillLevel level = mapProtoToEntityLevel(request.getLevel());

            ApplicantSkill applicantSkill = new ApplicantSkill(applicant, skill, level);
            ApplicantSkill saved = applicantSkillRepository.save(applicantSkill);


            ApplicantSkillResponse response = ApplicantSkillResponse.newBuilder()
                    .setId(saved.getId())
                    .setApplicantId(applicant.getId())
                    .setSkillId(skill.getId())
                    .setSkillName(skill.getName())
                    .setLevel(mapEntityToProtoLevel(saved.getLevel()))
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            e.printStackTrace();

            responseObserver.onError(
                    io.grpc.Status.INTERNAL
                            .withDescription(e.getMessage() != null ? e.getMessage() : e.getClass().getSimpleName())
                            .withCause(e)
                            .asRuntimeException());
        }
    }

    @Override
    @Transactional
    public void getApplicantSkills(GetApplicantSkillsRequest request,
                                   StreamObserver<ApplicantSkillsResponse> responseObserver) {
        try {
            List<ApplicantSkill> skills = applicantSkillRepository.findByApplicant_Id(request.getApplicantId());
            if(skills.isEmpty()) {
                throw new RuntimeException("Applicant with id " + request.getApplicantId() + " has no skills");
            }

            List<ApplicantSkillResponse> skillsResponse = new ArrayList<>();
            for(ApplicantSkill skill : skills) {
                skillsResponse.add(ApplicantSkillResponse.newBuilder()
                        .setId(skill.getId())
                        .setApplicantId(request.getApplicantId())
                        .setSkillId(skill.getSkill().getId())
                        .setSkillName(skill.getSkill().getName())
                        .setLevel(mapEntityToProtoLevel(skill.getLevel()))
                        .build());
            }
            ApplicantSkillsResponse response = ApplicantSkillsResponse.newBuilder()
                    .addAllSkills(skillsResponse)
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            e.printStackTrace();

            responseObserver.onError(
                    io.grpc.Status.INTERNAL
                            .withDescription(e.getMessage() != null ? e.getMessage() : e.getClass().getSimpleName())
                            .withCause(e)
                            .asRuntimeException());
        }
    }

    @Override
    @Transactional
    public void removeApplicantSkill(RemoveApplicantSkillRequest request,
                                     StreamObserver<RemoveApplicantSkillResponse> responseObserver) {
        try {
            long skillId = request.getApplicantSkillId();
            boolean exists = applicantSkillRepository.existsById(skillId);

            if (!exists) {
                RemoveApplicantSkillResponse response = RemoveApplicantSkillResponse.newBuilder()
                        .setSuccess(false)
                        .setMessage("Applicant skill not found.")
                        .build();
                responseObserver.onNext(response);
                responseObserver.onCompleted();
                return;
            }

            applicantSkillRepository.deleteById(skillId);

            RemoveApplicantSkillResponse response = RemoveApplicantSkillResponse.newBuilder()
                    .setSuccess(true)
                    .setMessage("Skill removed.")
                    .build();
            responseObserver.onNext(response);
            responseObserver.onCompleted();
        } catch (Exception e) {
            responseObserver.onError(
                    io.grpc.Status.INTERNAL
                            .withDescription(e.getMessage() != null ? e.getMessage() : e.getClass().getSimpleName())
                            .withCause(e)
                            .asRuntimeException());
        }
    }

    @Override
    @Transactional
    public void getApplicantById(GetApplicantRequest request,
                                   StreamObserver<ApplicantResponse> responseObserver) {
        try {
            Optional<Applicant> applicant = applicantRepository.findById(request.getId());
            if(applicant.isEmpty()) {
                responseObserver.onError(
                        io.grpc.Status.NOT_FOUND
                                .withDescription("No applicant with id: " + request.getId() + " found")
                                .asRuntimeException());
                return;
            }
            ApplicantResponse response = ApplicantResponse.newBuilder()
                    .setId(applicant.get().getId())
                    .setName(applicant.get().getUser().getName())
                    .setEmail(applicant.get().getUser().getEmail())
                    .setExperience(
                            applicant.get().getExperience() == null
                                    ? ""
                                    : applicant.get().getExperience()
                    )
                    .setCity(applicant.get().getLocation().getCity())
                    .setPostcode(
                            applicant.get().getLocation().getPostcode() == null
                                    ? ""
                                    : applicant.get().getLocation().getPostcode()
                    )
                    .setAddress(
                            applicant.get().getLocation().getAddress() == null
                                    ? ""
                                    : applicant.get().getLocation().getAddress()
                    )
                    .build();
            responseObserver.onNext(response);
            responseObserver.onCompleted();
        }
        catch (Exception e) {
            e.printStackTrace();

            responseObserver.onError(
                    io.grpc.Status.INTERNAL
                            .withDescription(e.getMessage() != null ? e.getMessage() : e.getClass().getSimpleName())
                            .withCause(e)
                            .asRuntimeException());
        }
    }

    private SkillLevel mapProtoToEntityLevel(SkillLevelProto protoLevel) {
        return switch (protoLevel) {
            case SKILL_LEVEL_BEGINNER -> SkillLevel.beginner;
            case SKILL_LEVEL_JUNIOR   -> SkillLevel.junior;
            case SKILL_LEVEL_MID      -> SkillLevel.mid;
            case SKILL_LEVEL_SENIOR   -> SkillLevel.senior;
            case SKILL_LEVEL_EXPERT   -> SkillLevel.expert;
            case UNRECOGNIZED, SKILL_LEVEL_UNSPECIFIED -> SkillLevel.beginner;
        };
    }

    private SkillLevelProto mapEntityToProtoLevel(SkillLevel level) {
        return switch (level) {
            case beginner -> SkillLevelProto.SKILL_LEVEL_BEGINNER;
            case junior   -> SkillLevelProto.SKILL_LEVEL_JUNIOR;
            case mid      -> SkillLevelProto.SKILL_LEVEL_MID;
            case senior   -> SkillLevelProto.SKILL_LEVEL_SENIOR;
            case expert   -> SkillLevelProto.SKILL_LEVEL_EXPERT;
        };
    }

    @Override
    @Transactional
    public void updateApplicant(UpdateApplicantRequest request,
                                StreamObserver<ApplicantResponse> responseObserver) {
        try {
            Applicant applicant = applicantRepository.findById(request.getId())
                    .orElseThrow(() -> new RuntimeException("Applicant not found with id: " + request.getId()));

            User user = applicant.getUser();

            if (!user.getEmail().equals(request.getEmail()) && userRepository.existsByEmail(request.getEmail())) {
                responseObserver.onError(
                        Status.ALREADY_EXISTS
                                .withDescription("Email already in use: " + request.getEmail())
                                .asRuntimeException()
                );
                return;
            }

            user.setName(request.getName());
            user.setEmail(request.getEmail());

            applicant.setExperience(request.getExperience());

            Location location = applicant.getLocation();
            if (location == null) {
                location = new Location(request.getCity(), request.getPostcode(), request.getAddress());
            } else {
                location.setCity(request.getCity());
                location.setPostcode(request.getPostcode());
                location.setAddress(request.getAddress());
            }

            Location savedLocation = locationRepository.save(location);
            applicant.setLocation(savedLocation);

            userRepository.save(user);
            applicantRepository.save(applicant);

            ApplicantResponse response = ApplicantResponse.newBuilder()
                    .setId(applicant.getId())
                    .setName(user.getName())
                    .setEmail(user.getEmail())
                    .setExperience(applicant.getExperience() == null ? "" : applicant.getExperience())
                    .setCity(savedLocation.getCity())
                    .setPostcode(savedLocation.getPostcode() == null ? "" : savedLocation.getPostcode())
                    .setAddress(savedLocation.getAddress() == null ? "" : savedLocation.getAddress())
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();
        } catch (Exception e) {
            e.printStackTrace();
            responseObserver.onError(
                    Status.INTERNAL
                            .withDescription(e.getMessage())
                            .withCause(e)
                            .asRuntimeException()
            );
        }
    }


}
