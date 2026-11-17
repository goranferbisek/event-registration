import { Link, useLoaderData, useSearchParams } from 'react-router'
import type { LoaderFunctionArgs } from 'react-router'
import { loadEvent, registrationClosedReason } from '../api'
import { formatStart } from '../dates'

export async function loader({ params }: LoaderFunctionArgs) {
  return loadEvent(Number(params.id))
}

export function EventDetailsPage() {
  const event = useLoaderData<typeof loader>()
  const [searchParams] = useSearchParams()
  const closedReason = registrationClosedReason(event)

  return (
    <article className="space-y-6">
      <Link to="/" className="inline-block rounded text-sm text-indigo-700 hover:underline focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500">
        ← Back to events
      </Link>

      {searchParams.get('registered') === '1' && (
        <p role="status" className="rounded-md bg-green-50 p-3 text-green-800 ring-1 ring-green-600/20">
          You are registered for this event.
        </p>
      )}

      <div className="space-y-6 rounded-lg bg-white p-6 shadow-sm">
        <h1 className="text-2xl font-semibold">{event.name}</h1>

        <dl className="grid gap-4 sm:grid-cols-2">
          <div>
            <dt className="text-sm text-slate-500">Date and time</dt>
            <dd className="font-medium">{formatStart(event.startDateTime)}</dd>
          </div>
          <div>
            <dt className="text-sm text-slate-500">Location</dt>
            <dd className="font-medium">{event.location}</dd>
          </div>
          <div>
            <dt className="text-sm text-slate-500">Capacity</dt>
            <dd className="font-medium">{event.capacity}</dd>
          </div>
          <div>
            <dt className="text-sm text-slate-500">Free spots</dt>
            <dd className="font-medium">{event.freeSpots}</dd>
          </div>
        </dl>

        {closedReason ? (
          <div className="flex items-center gap-3">
            <button type="button" disabled className="cursor-not-allowed rounded-md bg-slate-300 px-4 py-2 font-medium text-slate-600">
              Register
            </button>
            <span className="text-sm text-slate-600">{closedReason}</span>
          </div>
        ) : (
          <Link to={`/events/${event.id}/register`} className="inline-block rounded-md bg-indigo-600 px-4 py-2 font-medium text-white hover:bg-indigo-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500">
            Register
          </Link>
        )}
      </div>
    </article>
  )
}
