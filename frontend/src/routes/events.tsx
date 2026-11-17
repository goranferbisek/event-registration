import { Form, Link, useLoaderData, useNavigation } from 'react-router'
import type { LoaderFunctionArgs } from 'react-router'
import { getEvents } from '../api'
import type { EventSummary } from '../api'
import { endOfDayIso, formatStart, parseDay, startOfDayIso } from '../dates'

export async function loader({ request }: LoaderFunctionArgs) {
  const params = new URL(request.url).searchParams
  const from = parseDay(params.get('from'))
  const to = parseDay(params.get('to'))

  if (from && to && from > to) {
    return { from, to, events: [] as EventSummary[], filterError: "'From' must be on or before 'To'." }
  }

  const events = await getEvents(from ? startOfDayIso(from) : undefined, to ? endOfDayIso(to) : undefined)
  return { from, to, events, filterError: null }
}

const inputClass = 'rounded-md border border-slate-300 px-3 py-2 focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500'

export function EventsPage() {
  const { from, to, events, filterError } = useLoaderData<typeof loader>()
  const navigation = useNavigation()
  const loading = navigation.state === 'loading'

  return (
    <section className="space-y-6">
      <Form key={`${from}|${to}`} method="get" className="flex flex-wrap items-end gap-3 rounded-lg bg-white p-4 shadow-sm">
        <label className="flex flex-col gap-1 text-sm font-medium text-slate-700">
          From
          <input type="date" name="from" defaultValue={from} className={inputClass} />
        </label>
        <label className="flex flex-col gap-1 text-sm font-medium text-slate-700">
          To
          <input type="date" name="to" defaultValue={to} className={inputClass} />
        </label>
        <button type="submit" className="rounded-md bg-indigo-600 px-4 py-2 font-medium text-white hover:bg-indigo-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500">
          Filter
        </button>
        <Link to="/" className="rounded-md px-3 py-2 text-indigo-700 hover:underline focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500">
          Clear
        </Link>
      </Form>

      {filterError && (
        <p role="alert" className="rounded-md bg-red-50 p-3 text-sm text-red-700 ring-1 ring-red-600/20">
          {filterError}
        </p>
      )}

      {!filterError && events.length === 0 && (
        <p className="text-slate-600">No events in this date range.</p>
      )}

      {!filterError && events.length > 0 && (
        <ul className={`space-y-3 transition-opacity ${loading ? 'opacity-60' : ''}`}>
          {events.map((event) => {
            const spotsLeft = Math.max(0, event.capacity - event.registrationCount)
            return (
              <li key={event.id} className="rounded-lg bg-white p-4 shadow-sm">
                <Link to={`/events/${event.id}`} className="rounded text-lg font-semibold text-indigo-700 hover:underline focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500">
                  {event.name}
                </Link>
                <p className="text-sm text-slate-600">{formatStart(event.startDateTime)}</p>
                <p className="text-sm text-slate-600">{event.location}</p>
                <p className="mt-1 text-sm">
                  {spotsLeft === 0
                    ? <span className="font-medium text-red-600">Full</span>
                    : <span className="text-slate-700">{`${spotsLeft} of ${event.capacity} spots left`}</span>}
                </p>
              </li>
            )
          })}
        </ul>
      )}
    </section>
  )
}
