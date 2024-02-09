// JavaScript source code

var Sdk = window.Sdk || {};
    (function () {
        this.companyOnChange = function (executionContext) {
            let formContext = executionContext.getFormContext();
            let customerArray = formContext.getAttribute("parentcustomerid").getValue();

            if (customerArray && customerArray.length && customerArray[0]) {
                var customerGuid = customerArray[0].id;
                var customerType = customerArray[0].entityType

                // console.log(customerGuid, customerType);

                Xrm.WebApi.retrieveRecord(customerType, customerGuid, "?$select=telephone1,emailaddress1").then(
                    function success(result) {
                        // console.log("Retrieved values: Phone: " + result.telephone1 + ", Email: " + result.emailaddress1);
                        
                        formContext.getAttribute("telephone1").setValue(result.telephone1)
                        formContext.getAttribute("emailaddress1").setValue(result.emailaddress1)
                    },
                    function (error) {
                        console.log(error.message);
                        // handle error conditions
                    }
                );
            }

        }
    }).call(Sdk)
