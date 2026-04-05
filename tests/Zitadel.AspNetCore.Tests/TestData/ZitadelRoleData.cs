namespace Zitadel.AspNetCore.Tests.TestData;

public class ZitadelRoleData
{
    public const string AdminAndViewerRoles = """
                                              {
                                                "admin" : {
                                                  "334771488802539102" : "acme.sh"
                                                },
                                                "viewer" : {
                                                  "334771488802539102": "acme.sh",
                                                  "727577719387174842": "zitadel.cloud"
                                                }
                                              }
                                              """;

    public const string ViewerRole = """
                                     {
                                       "viewer" : {
                                         "334771488802539102": "acme.sh",
                                         "727577719387174842": "zitadel.cloud"
                                       }
                                     }
                                     """;
}
