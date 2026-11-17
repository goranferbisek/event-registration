import { Link, isRouteErrorResponse, useRouteError } from 'react-router'

export function ErrorPage() {
  const error = useRouteError()
  const notFound = isRouteErrorResponse(error) && error.status === 404

  return (
    <div className="mx-auto max-w-md space-y-4 px-4 py-16 text-center">
      <h1 className="text-2xl font-semibold">{notFound ? 'Not found' : 'Something went wrong'}</h1>
      <p className="text-slate-600">
        {notFound
          ? "The page or event you're looking for doesn't exist."
          : 'An unexpected error occurred. Please try again.'}
      </p>
      <Link to="/" className="inline-block rounded-md bg-indigo-600 px-4 py-2 text-white hover:bg-indigo-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500">
        Back to events
      </Link>
    </div>
  )
}
