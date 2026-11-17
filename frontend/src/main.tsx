import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createBrowserRouter } from 'react-router'
import { RouterProvider } from 'react-router/dom'
import './index.css'
import { ErrorPage } from './routes/error'
import { EventDetailsPage, loader as eventDetailsLoader } from './routes/event-details'
import { EventsPage, loader as eventsLoader } from './routes/events'
import { RegisterPage, action as registerAction, loader as registerLoader } from './routes/register'
import { RootLayout } from './routes/root'

const router = createBrowserRouter([
  {
    path: '/',
    Component: RootLayout,
    errorElement: <ErrorPage />,
    children: [
      { index: true, loader: eventsLoader, Component: EventsPage },
      { path: 'events/:id', loader: eventDetailsLoader, Component: EventDetailsPage },
      {
        path: 'events/:id/register',
        loader: registerLoader,
        action: registerAction,
        Component: RegisterPage,
      },
    ],
  },
])

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
)
