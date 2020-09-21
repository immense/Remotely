import { MainApp } from "./Main/App";
import { ViewerApp } from "./RemoteControl/App";
import { ApiTokens } from "./Pages/ApiTokens";
import { IndexNotLoggedIn } from "./Pages/IndexNotLoggedIn";
import { OrganizationManagement } from "./Pages/OrganizationManagement";
import { ServerConfig } from "./Pages/ServerConfig";

export const App = {
    Main: MainApp,
    Viewer: ViewerApp,
    Pages: {
        ApiTokens: ApiTokens,
        IndexNotLoggedIn: IndexNotLoggedIn,
        OrganizationManagement: OrganizationManagement,
        ServerConfig: ServerConfig
    }
}