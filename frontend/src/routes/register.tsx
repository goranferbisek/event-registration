import { Form, Link, data, redirect, useActionData, useLoaderData, useNavigation } from 'react-router'
import type { ActionFunctionArgs, LoaderFunctionArgs } from 'react-router'
import { ApiError, createRegistration, loadEvent, registrationClosedReason } from '../api'
import type { RegistrationRequest } from '../api'
import { formatStart } from '../dates'

type Field = keyof RegistrationRequest
type FieldErrors = Partial<Record<Field, string>>

interface RegisterResult {
  values: RegistrationRequest
  fieldErrors?: FieldErrors
  formError?: string
}

const FIELD_KEYS = new Map<string, Field>([
  ['fullname', 'fullName'],
  ['email', 'email'],
])
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

function validate({ fullName, email }: RegistrationRequest): FieldErrors | null {
  const errors: FieldErrors = {}
  if (!fullName) errors.fullName = 'Full name is required.'
  else if (fullName.length > 200) errors.fullName = 'Full name must be 200 characters or fewer.'
  if (!email) errors.email = 'Email is required.'
  else if (email.length > 254) errors.email = 'Email must be 254 characters or fewer.'
  else if (!EMAIL_PATTERN.test(email)) errors.email = 'Enter a valid email address.'
  return Object.keys(errors).length > 0 ? errors : null
}

export async function loader({ params }: LoaderFunctionArgs) {
  return loadEvent(Number(params.id))
}

export async function action({ request, params }: ActionFunctionArgs) {
  const eventId = Number(params.id)
  const form = await request.formData()
  const values: RegistrationRequest = {
    fullName: String(form.get('fullName') ?? '').trim(),
    email: String(form.get('email') ?? '').trim(),
  }

  const clientErrors = validate(values)
  if (clientErrors) return data<RegisterResult>({ values, fieldErrors: clientErrors }, { status: 400 })

  try {
    await createRegistration(eventId, values)
  } catch (error) {
    if (error instanceof TypeError) {
      return data<RegisterResult>(
        { values, formError: 'Could not reach the server. Please try again.' },
        { status: 503 },
      )
    }
    if (error instanceof ApiError) {
      const validation = error.problem.errors ?? {}
      const fieldErrors: FieldErrors = {}
      let formError: string | undefined
      for (const [key, messages] of Object.entries(validation)) {
        const field = FIELD_KEYS.get(key.toLowerCase())
        if (field) fieldErrors[field] ??= messages[0]
        else formError ??= messages[0]
      }
      if (Object.keys(validation).length === 0) {
        formError = error.problem.detail ?? error.problem.title
      }
      return data<RegisterResult>({ values, fieldErrors, formError }, { status: error.status })
    }
    throw error
  }

  return redirect(`/events/${eventId}?registered=1`)
}

interface TextFieldProps {
  name: Field
  label: string
  type: string
  defaultValue?: string
  error?: string
}

function TextField({ name, label, type, defaultValue, error }: TextFieldProps) {
  const errorId = `${name}-error`
  return (
    <div className="space-y-1">
      <label htmlFor={name} className="block text-sm font-medium text-slate-700">
        {label}
      </label>
      <input
        id={name}
        name={name}
        type={type}
        defaultValue={defaultValue}
        aria-invalid={error ? true : undefined}
        aria-describedby={error ? errorId : undefined}
        className={`w-full rounded-md border px-3 py-2 focus:outline-none focus-visible:ring-2 ${
          error ? 'border-red-500 focus-visible:ring-red-500' : 'border-slate-300 focus-visible:ring-indigo-500'
        }`}
      />
      {error && <p id={errorId} className="text-sm text-red-600">{error}</p>}
    </div>
  )
}

export function RegisterPage() {
  const event = useLoaderData<typeof loader>()
  const result = useActionData<typeof action>() as RegisterResult | undefined
  const navigation = useNavigation()
  const submitting = navigation.state === 'submitting'
  const closedReason = registrationClosedReason(event)

  return (
    <section className="mx-auto max-w-lg space-y-6">
      <Link to={`/events/${event.id}`} className="inline-block rounded text-sm text-indigo-700 hover:underline focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500">
        ← Back to event
      </Link>

      <div className="space-y-6 rounded-lg bg-white p-6 shadow-sm">
        <div className="space-y-1">
          <h1 className="text-2xl font-semibold">Register for {event.name}</h1>
          <p className="text-sm text-slate-600">{formatStart(event.startDateTime)}</p>
        </div>

        {closedReason ? (
          <p className="rounded-md bg-slate-100 p-3 text-slate-700">{closedReason}. You can no longer register for this event.</p>
        ) : (
          <Form method="post" noValidate className="space-y-4">
            {result?.formError && (
              <div role="alert" className="rounded-md bg-red-50 p-3 text-sm text-red-700 ring-1 ring-red-600/20">
                {result.formError}
              </div>
            )}

            <TextField
              name="fullName"
              label="Full name"
              type="text"
              defaultValue={result?.values.fullName}
              error={result?.fieldErrors?.fullName}
            />
            <TextField
              name="email"
              label="Email"
              type="email"
              defaultValue={result?.values.email}
              error={result?.fieldErrors?.email}
            />

            <div className="flex items-center gap-3 pt-2">
              <button
                type="submit"
                disabled={submitting}
                className="rounded-md bg-indigo-600 px-4 py-2 font-medium text-white hover:bg-indigo-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {submitting ? 'Registering…' : 'Register'}
              </button>
              <Link to={`/events/${event.id}`} className="rounded-md px-3 py-2 text-slate-700 hover:underline focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-500">
                Cancel
              </Link>
            </div>
          </Form>
        )}
      </div>
    </section>
  )
}
