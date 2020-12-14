export const ApiTokens = {
    Init() {
        document.querySelectorAll(".delete-form").forEach((deleteForm: HTMLFormElement) => {
            deleteForm.addEventListener("submit", ev => {
                var result = confirm("Are you sure you want to delete this API token?");
                if (result) {
                    deleteForm.removeEventListener("submit", undefined);
                    deleteForm.submit();
                }
            });
        });

        document.querySelectorAll(".rename-form").forEach((renameForm: HTMLFormElement) => {
            renameForm.addEventListener("submit", ev => {
                var result = prompt("Enter a new name for the token");
                if (result) {
                    (renameForm.querySelector('input[name="Input.TokenName"]') as HTMLInputElement).value = result;
                    renameForm.removeEventListener("submit", undefined);
                    renameForm.submit();
                }
            });
        });
    }
}