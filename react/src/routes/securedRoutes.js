import { lazy } from "react";
const AnalyticsDashboards = lazy(() =>
  import("../components/dashboard/analytics/Analytics")
);
const AdminDashboard = lazy(() =>
  import("../components/dashboard/admin/AdminDashboard")
);
const OrgDashboard = lazy(() =>
  import("../components/dashboard/organization/OrgDashboard")
);
const OrgMemberDashboard = lazy(() =>
  import("../components/dashboard/orgmember/OrgMemberDashboard")
);
const BlogArticleSingle = lazy(() =>
  import("../components/blogs/BlogArticleSingle")
);
const Users = lazy(() =>
  import("../components/dashboard/admin/userstatus/Users")
);

const BlogForm = lazy(() => import("../components/blogs/BlogForm"));
const BlogListing = lazy(() => import("../components/blogs/BlogListing"));
const AddCharity = lazy(() => import("../components/charities/CharityForm"));
const StoryApproval = lazy(() =>
  import("../components/storyapproval/StoryApproval")
);
const UserDashboard = lazy(() =>
  import("../components/dashboard/user/UserDashboard")
);
const OrganizationSingleList = lazy(() =>
  import("../components/organizations/OrganizationSingleList")
);
const SiteReferenceChart = lazy(() =>
  import("../components/sitereference/SiteReferenceChart")
);
const SharedStory = lazy(() => import("../components/sharedstory/SharedStory"));
const ExternalLink = lazy(() =>
  import("../components/externallinks/ExternalLinks")
);
const PageNotFound = lazy(() => import("../components/error/Error404"));
const FileManager = lazy(() => import("../components/filemanager/FileManager"));

const HealthLog = lazy(() =>
  import("../components/healthprofile/healthlogs/HealthLog")
);
const Charities = lazy(() => import("../components/charities/CharitiesPage"));
const Podcasts = lazy(() => import("../components/podcasts/Podcasts"));
const HealthProfileWizard = lazy(() =>
  import("../components/healthprofile/HealthProfileWizard")
);
const Locations = lazy(() => import("../components/locations/Locations"));
const LocationForm = lazy(() => import("../components/locations/LocationForm"));
const UserSettings = lazy(() =>
  import("../components/usersettings/UserSettings")
);
const JobForm = lazy(() => import("../components/jobs/JobForm"));
const VenueForm = lazy(() => import("../components/venues/VenueForm"));
const ChatPage = lazy(() => import("../components/chat/MainChatPage"));
const OrgJobPosts = lazy(() => import("../components/jobs/OrgJobPosts"));
const EventForm = lazy(() => import("../components/events/EventForm"));
const VenueListing = lazy(() => import("../components/venues/VenueListing"));
const CourseCreate = lazy(() => import("components/courses/CourseForm"));
const Comments = lazy(() => import("../components/comment/Comments"));
const AppointmentForm = lazy(() =>
  import("../components/appointments/AppointmentForm")
);
const Lectures = lazy(() => import("../components/lectures/LectureForm"));
const CalendarApp = lazy(() =>
  import("../components/appointments/CalenderApp")
);
const AssignCoursesForm = lazy(() =>
  import("../components/educationhub/AssignCoursesForm")
);
const MyCourse = lazy(() => import("components/educationhub/MyCourse"));
const NotesForm = lazy(() => import("components/notes/NotesForm"));
const Notes = lazy(() => import("components/notes/Notes"));
const CourseDetails = lazy(() =>
  import("components/coursedetails/CourseDetails")
);
const EventSingleListing = lazy(() =>
  import("../components/events/EventSingleListing")
);
const dashboardRoutes = [
  {
    path: "/dashboard",
    name: "Dashboards",
    icon: "uil-home-alt",
    header: "Navigation",
    children: [
      {
        path: "/dashboard/analytics",
        name: "Analytics",
        element: AnalyticsDashboards,
        roles: ["Admin"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/dashboard/admin",
        name: "Admin Dashboard",
        element: AdminDashboard,
        roles: ["Admin"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/charities/add",
        name: "AddCharity",
        exact: true,
        element: AddCharity,
        roles: ["Admin"],
        isAnonymous: true,
      },
      {
        path: "/course/create",
        name: "CourseCreate",
        exact: true,
        element: CourseCreate,
        roles: ["Admin", "Organization"],
        isAnonymous: true,
      },
      {
        path: "/course/edit/:id",
        name: "CourseEdit",
        exact: true,
        element: CourseCreate,
        roles: ["Admin", "Organization"],
        isAnonymous: true,
      },
      {
        path: "/charities/edit/:id",
        name: "EditCharity",
        exact: true,
        element: AddCharity,
        roles: ["Admin"],
        isAnonymous: true,
      },
      {
        path: "/dashboard/user",
        name: "User Dashboard",
        element: UserDashboard,
        roles: ["User"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/dashboard/organization",
        name: "Organization Dashboard",
        element: OrgDashboard,
        roles: ["Organization"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/organization/single/:id",
        name: "OrganizationSingleListing",
        exact: true,
        element: OrganizationSingleList,
        roles: [],
        isAnonymous: false,
      },
      {
        path: "/dashboard/orgmember",
        name: "Org Member Dashboard",
        element: OrgMemberDashboard,
        roles: ["Org Member"],
        exact: true,
        isAnonymous: false,
      },

      {
        path: "/comments",
        name: "Comments",
        element: Comments,
        roles: [],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/appointment",
        name: "Appointment",
        exact: true,
        element: AppointmentForm,
        roles: ["Organization"],
        isAnonymous: true,
      },
      {
        path: "/calendar",
        name: "Calendar",
        exact: true,
        element: CalendarApp,
        roles: ["Org Member", "Organization"],
        isAnonymous: true,
      },
      {
        path: "/blog/create",
        name: "Create Blog",
        element: BlogForm,
        roles: ["Admin", "Org Member", "Organization"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "blog/listings",
        name: "Blog",
        exact: true,
        element: BlogListing,
        roles: [],
        isAnonymous: false,
      },
      {
        path: "/story/create",
        name: "Create Story",
        exact: true,
        element: SharedStory,
        roles: [],
        isAnonymous: false,
      },
      {
        path: "/story/approval",
        name: "Story Approval",
        exact: true,
        element: StoryApproval,
        roles: ["Admin"],
        isAnonymous: false,
      },
      {
        path: "/sitereference/chart",
        name: "sitereferencechart",
        exact: true,
        element: SiteReferenceChart,
        roles: ["Admin"],
        isAnonymous: false,
      },
      {
        path: "/dashboard/settings",
        name: "User Settings",
        element: UserSettings,
        roles: ["User"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/article/:id",
        name: "Blog",
        exact: true,
        element: BlogArticleSingle,
        roles: [],
        isAnonymous: true,
      },
      {
        path: "/user/initial/setup",
        name: "HealthProfileWizard",
        exact: true,
        element: HealthProfileWizard,
        roles: ["Admin", "Org Member", "User"],
        isAnonymous: true,
      },
      {
        path: "/podcasts",
        name: "Podcasts",
        exact: true,
        element: Podcasts,
        roles: [],
        isAnonymous: true,
      },
      {
        path: "/locations",
        name: "Location",
        exact: true,
        element: Locations,
        roles: ["Admin", "Organization"],
        isAnonymous: true,
      },
      {
        path: "/location/form",
        name: "Add Location",
        exact: true,
        element: LocationForm,
        roles: ["Admin", "Organization"],
        isAnonymous: true,
      },
      {
        path: "/healthlog",
        name: "Health Logs",
        exact: true,
        element: HealthLog,
        roles: ["Admin", "User"],
        isAnonymous: false,
      },
      {
        path: "/charitiess",
        name: "Charities",
        exact: true,
        element: Charities,
        roles: [],
        isAnonymous: true,
      },
      {
        path: "/location/:id/edit",
        name: "Edit Location",
        exact: true,
        element: LocationForm,
        roles: ["Admin", "Organization"],
        isAnonymous: true,
      },
      {
        path: "/jobs/new",
        name: "New Job",
        element: JobForm,
        roles: ["Organization"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/jobs/:id",
        name: "Edit Job",
        element: JobForm,
        roles: ["Organization"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/organization/jobposts",
        name: "Org Job Posts",
        element: OrgJobPosts,
        roles: ["Organization"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/venue/create",
        name: "VenueForm",
        exact: true,
        element: VenueForm,
        roles: ["Admin", "Organization"],
        isAnonymous: true,
      },
      {
        path: "/venue/:id/edit",
        name: "VenueEdit",
        exact: true,
        element: VenueForm,
        roles: ["Admin", "Organization"],
        isAnonymous: true,
      },
      {
        path: "/venue/listings",
        name: "VenueListing",
        exact: true,
        element: VenueListing,
        roles: [],
        isAnonymous: false,
      },

      {
        path: "dashboard/admin/status-list",
        name: "Status List",
        exact: true,
        element: Users,
        roles: ["Admin"],
        isAnonymous: true,
      },
      {
        path: "/events/new",
        name: "New Event",
        element: EventForm,
        roles: ["Admin", "Organization"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/lectures/create",
        name: "Lectures",
        element: Lectures,
        roles: ["Admin", "Organization"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/events/edit/:id",
        name: "Edit Event",
        element: EventForm,
        roles: ["Organization", "Admin"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/course/assign",
        name: "Assign Course",
        element: AssignCoursesForm,
        roles: ["Organization"],
        exact: true,
        isAnonymous: false,
      },
      {
        path: "/mycourses",
        name: "MyCourse",
        exact: true,
        element: MyCourse,
        roles: ["Org Member", "Organization"],
        isAnonymous: false,
      },
      {
        path: "/notes/create",
        name: "Notes",
        exact: true,
        element: NotesForm,
        roles: ["User", "Org Memeber"],
        isAnonymous: true,
      },
      {
        path: "/notes/edit/:id",
        name: "Notes",
        exact: true,
        element: NotesForm,
        roles: ["User", "Org Memeber"],
        isAnonymous: true,
      },
      {
        path: "notes",
        name: "Notes",
        exact: true,
        element: Notes,
        roles: ["User", "Org Memeber"],
        isAnonymous: true,
      },
      {
        path: "/course/:id/details",
        name: "CourseDetails",
        exact: true,
        element: CourseDetails,
        roles: [],
        isAnonymous: true,
      },
      {
        path: "/event/:id/details",
        name: "Event Details",
        exact: true,
        element: EventSingleListing,
        roles: [],
        isAnonymous: true,
      },
    ],
  },
];

const test = [
  {
    path: "/test",
    name: "Test",
    exact: true,
    element: AnalyticsDashboards,
    roles: ["Fail"],
    isAnonymous: false,
  },
  {
    path: "/secured",
    name: "A Secured Route",
    exact: true,
    element: AnalyticsDashboards,
    roles: ["Fail"],
    isAnonymous: false,
  },
  {
    path: "/secured2",
    name: "A Secured Route",
    exact: true,
    element: AnalyticsDashboards,
    roles: ["Admin"],
    isAnonymous: false,
  },
];

const fileRoutes = [
  {
    path: "/files",
    name: "FileManager",
    exact: true,
    element: FileManager,
    roles: [],
    isAnonymous: true,
  },
  {
    path: "/external",
    name: "External Link",
    exact: true,
    element: ExternalLink,
    roles: ["User"],
    isAnonymous: false,
  },

  {
    path: "/chat",
    name: "Chat",
    exact: true,
    element: ChatPage,
    roles: ["Admin", "Org Member", "User", "Organization"],
    isAnonymous: false,
  },
];

const errorRoutes = [
  {
    path: "*",
    name: "Error - 404",
    element: PageNotFound,
    roles: [],
    exact: true,
    isAnonymous: false,
  },
];

const allRoutes = [...dashboardRoutes, ...test, ...fileRoutes, ...errorRoutes];

export default allRoutes;
