// JavaScript source code

var Sdk = window.Sdk || {};
    (function () {
        this.companyOnChange = function (executionContext) {
            let formContext = executionContext.getFormContext();
            let customerArray = formContext.getAttribute("parentcustomerid").getValue();

            if (customerArray.length && customerArray[0]) {
                var customerGuid = customerArray[0].id;
            }

            Xrm.WebApi.retrieveRecord("account", customerGuid, "?$select=name,revenue").then(
                function success(result) {
                    console.log("Retrieved values: Name: " + result.name + ", Revenue: " + result.revenue);
                    // perform operations on record retrieval
                },
                function (error) {
                    console.log(error.message);
                    // handle error conditions
                }
            );
        }
    }).call(Sdk)
