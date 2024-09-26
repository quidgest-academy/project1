using CSGenio.business;
using CSGenio.framework;
using NUnit.Framework;
using Quidgest.Persistence.GenericQuery;

namespace DbAdmin.IntegrationTest
{
    public class UserTableCrudTests : DatabaseTransactionFixture
    {
        [Test]
        public void InsertUser()
        {
            var codpsw = InsertTestUser();

            Assert.AreEqual(0, emptyKey(codpsw));
        }

        private string InsertTestUser()
        {
            CSGenioApsw newUser = new CSGenioApsw(_user);
            newUser.ValNome = "IntegrationTester";
            newUser.insert(sp);
            return newUser.ValCodpsw;
        }

        private void AssignRoleToUser(CSGenioApsw user, string module, Role role)
        {
            CSGenioAs_ua userAuthorization = new CSGenioAs_ua(_user)
            {
                ValSistema = "TST",
                ValModulo = module,
                ValRole = role.Id,
                ValCodpsw = user.ValCodpsw
            };

            userAuthorization.insert(sp);
        }

        [Test]
        public void ReadUser()
        {
            //Arrange
            var codpsw = InsertTestUser();
            //Act
            CSGenioApsw returnedUser = CSGenioApsw.search(sp, codpsw, _user);
            //Assert
            Assert.AreEqual(codpsw, returnedUser.ValCodpsw);
            Assert.AreEqual("IntegrationTester", returnedUser.ValNome);
        }

        [Test]
        public void EditUser()
        {
            //Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw returnedUser = CSGenioApsw.search(sp, codpsw, _user);

            //Act
            returnedUser.ValNome = "IntegrationTester2";
            returnedUser.update(sp);

            //Assert
            Assert.AreEqual(codpsw, returnedUser.ValCodpsw);
            Assert.AreEqual("IntegrationTester2", returnedUser.ValNome);
        }

        [Test]
        public void DeleteUser()
        {
            //Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw existingUser = CSGenioApsw.search(sp, codpsw, _user);

            //Act
            existingUser.delete(sp);

            //Assert
            CSGenioApsw searchedUser = CSGenioApsw.search(sp, codpsw, _user);
            Assert.IsNull(searchedUser);
        }

        [Test]
        public void InsertUserAuthorization()
        {
            //Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw user = CSGenioApsw.search(sp, codpsw, _user);
            Role role = Role.ADMINISTRATION;

            //Act
            AssignRoleToUser(user, "TBL", role);

            //Assert
            var roles = CSGenioAs_ua.searchList(sp, _user, CriteriaSet.And().Equal(CSGenioAs_ua.FldCodpsw, user.ValCodpsw));
            Assert.AreEqual(roles.Count, 1);
        }

        [Test]
        public void InsertDuplicateUserAuthorizationFails()
        {
            //Arrange
            var codpsw = InsertTestUser();
            CSGenioApsw user = CSGenioApsw.search(sp, codpsw, _user);
            Role role = Role.ADMINISTRATION;
            AssignRoleToUser(user, "TBL", role);

            //Act & Assert
            Assert.Throws<BusinessException>(() => AssignRoleToUser(user, "TBL", role));
        }

        public static int emptyKey(object characters)
        {
            if (
                characters == null
                || characters.Equals("")
                || characters.Equals(Guid.Empty.ToString())
                || characters.Equals(Guid.Empty.ToString("B"))
                || characters.Equals("0")
            )
                return 1;
            else
                return 0;
        }
    }
}
