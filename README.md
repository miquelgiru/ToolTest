# ToolTest
This is a test for a Senior Tool developer.
Unity Editor tool that lets the team create, manage, and configure test user
accounts and their associated data — all without leaving the Unity Editor.
The project uses Unity Gaming Services (UGS) for
player authentication and data persistence. Testing requires managing test users — player
accounts with specific configurations (level, currency, items, A/B test groups, etc.) that QA and
developers use daily to validate features.


*Architecture Overview: Diagram or description of layer separation (UI →
Business Logic → Services)*

To manage the UI Toolkit tool there is a Monobehaviour script called QAToolController. This scipt is attached to an object in the scene called QATool.
This script manages all the logic of the tool and gets the players information from the Monobehaviour PlayersDataManager which is in the same GameObject. 
The PlayersDataManager class manages the players data. Contains the service class which retrieves all the players data and tranforms it to usable structured data for the 
QAToolControlled to be used. The service class is called ToolTestService which inherits from an IToolTestService that all services (real & mock) must inherit. This class 
handles authentication and endpoint calls to both Unity services used. Finally this service send the WebRequests using a generic WebRequestClient.

QATool (UI) --> QAToolController --> PlayersDataManager --> ToolTestService(IToolTestService) --> WebRequestClient

*Key Design Decisions: Why you structured it this way, what trade-offs you made*

I tried to separate the UI layer, the data layer, and the service (infrastructure). So this way there are less dependencies between classes and you can retreive data 
from different services if needed. Added some simple injection pattern for different services. Also added some catching data system to try to do less calls.

There are some trade-offs to this implementation specially on the PlayerDataManager script. This class may handle too many things at once like data transformation, cache logic, service management.
The QAToolController is a bit too simple and could have better structure. Maybe add an state machine pattern to handle better the tool.

*Testing Strategy: What you tested, what you'd test with more time*

I have done basic tests of data input and output and its validation. This is a basic approach on the minimum required for this application.
For further tests, it would be great to handle corrupted data from services and tool possible exploits like changing tabs and storing data quick.

*Setup Instructions: How to run the tool and test*

To run the tool you just need to press play in the UnityEditor and it will start itself.
The tab "Players List" shows all the players data. Click one to be able to modify its data or delete it.
The tab "Create Player" is used to create a new player. You can select a preset from the left panel to add some data directly. To add an item just press the button add and it will
show a panel with the available items. Click one to add it.

To run the test just go to Windows->General->Test Runner and run the tests from ToolTest

*Known Limitations: What's missing, what you'd improve next*

The first and most important would be to remove the credentials from the Unity project itself and move it to another location so it is safer. It is wrong from the security part that this stays inside
the Unity project.
The second thig I would improve is the Tool itself. Add better visual, better display of the data and probably better usage design after some user testings and feedback.
It would be great also if the presets were loaded from adressables for example and you can add them on runtime or create new presets.
Finally, as I mentioned in the testing part, better tests and error messages inside the Tool not in the console errors.


*AI usage*

I have used ChatGPT and Claude. 
- First to make the first approach to how I will develop the tool.
- For the generation of tests.
- For small questions abut UI Toolkit (Callback events).
- To create the generic WebRequestClient.
- And finally, to improve my classes looking for possible errors.

*Documentation*

I have used the unity documentation:
- https://services.docs.unity.com/docs/service-account-auth/index.html
- Player Authentication Admin API: https://services.docs.unity.com/player-auth-admin/v1/index.html#tag/Player-Authentication-Admin/operation/GetPlayer
- CloudSave admin API: https://services.docs.unity.com/cloud-save-admin/v1/
